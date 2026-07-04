# Hotel EDA — Reservas de Hotel con Arquitectura Orientada a Eventos

Sistema de reservas de hotel implementado como microservicios independientes que colaboran
mediante mensajería asíncrona (RabbitMQ + MassTransit), coordinados por una **saga orquestada**.

## Caso de negocio

Un cliente solicita reservar una habitación de un hotel para un rango de fechas. Esa única
solicitud dispara, de forma automática y distribuida, tres pasos que hoy viven en tres servicios
distintos:

1. **Bloquear una habitación** disponible del hotel/tipo pedido (Inventory).
2. **Cobrar** el importe de la estadía (Payments).
3. **Confirmar** la reserva sólo si los dos pasos anteriores tuvieron éxito (Reservations).

Si no hay habitación disponible, la reserva se **rechaza** sin llegar a cobrar nada. Si hay
habitación pero el cobro falla, el sistema **compensa automáticamente**: libera la habitación que
había bloqueado y cancela la reserva. Ningún servicio conoce ni llama directamente a los otros dos:
todo se coordina por eventos a través del bus de mensajes.

## Arquitectura de la solución

Tres microservicios independientes (cada uno con su propia base de datos, dominio y ciclo de
vida) más una librería de contratos compartidos:

```
Hotel.Reservations   — dueño del proceso de negocio. Publica el evento que dispara todo
                        (ReservationRequested) y aloja la SAGA orquestadora.
Hotel.Inventory      — participante. Reacciona a comandos de la saga para bloquear/liberar
                        habitaciones.
Hotel.Payments       — participante. Reacciona a comandos de la saga para cobrar.
Hotel.Shared         — librería sin lógica: sólo los contratos (records) de los mensajes
                        que viajan por el bus, para que los 3 servicios hablen el mismo idioma
                        sin acoplarse entre sí (sin referencias de un servicio a otro).
```

Cada servicio sigue la misma organización interna (Clean Architecture / CQRS):

```
<Servicio>.Domain          Entidades, value objects, invariantes de negocio. Sin dependencias externas.
<Servicio>.Application      Comandos/Queries + Handlers (MediatR), DTOs, validadores (FluentValidation).
<Servicio>.Infrastructure   EF Core (AppDbContext, repositorios), configuración (RabbitMq, DB).
<Servicio>.Api              Controllers REST, Consumers de MassTransit, Program.cs (composition root).
                            Reservations.Api además aloja la SagaStateMachine.
```

### Por qué una saga orquestada (y no coreografía)

La orquestación centraliza el flujo y el estado ("¿en qué paso va esta reserva?") en un único
lugar (`Hotel.Reservations.Api/Sagas`), en vez de repartir ese conocimiento implícitamente entre
los `Reason`/`if` de varios consumers. Facilita razonar sobre las dos rutas de compensación y
evita que Inventory o Payments necesiten saber nada del otro.

### Diagrama de flujo

```
Cliente               Reservations              Inventory              Payments
  │  POST /Reservation      │                        │                       │
  ├─────────────────────────▶ Pending                │                       │
  │                          │  ReservationRequested  │                       │
  │                          ├───────────────────────▶│ (SAGA arranca)        │
  │                          │  CheckRoomAvailability  │                       │
  │                          ├────────────────────────▶ busca/bloquea habitación
  │                          │                        │                       │
  │                          │◀── RoomReserved ────────┤ (o RoomReservationFailed)
  │                          │      │                 │                       │
  │                          │      │ si falló: ReservationRejected → Rejected │
  │                          │      │ si ok: ProcessPayment                    │
  │                          │      ├──────────────────────────────────────────▶ cobra
  │                          │                        │                       │
  │                          │◀───────── PaymentCompleted / PaymentFailed ─────┤
  │                          │      │                 │                       │
  │                          │      │ si ok: ReservationConfirmed → Confirmed  │
  │                          │      │ si falló: ReleaseRoom (compensación) +   │
  │                          │      │           ReservationCancelled → Cancelled│
```

## Eventos de negocio (contratos en `Hotel.Shared/ServiceBusMessages`)

Todos correlacionan por `ReservationId` — es la clave con la que la saga sabe a qué instancia
en curso pertenece cada mensaje que llega.

| Mensaje | Publica | Consume | Rol |
|---|---|---|---|
| `ReservationRequested` | Reservations (al crear la reserva) | Saga | Dispara el proceso completo |
| `CheckRoomAvailability` | Saga | Inventory | Comando: pide bloquear una habitación |
| `RoomReserved` | Inventory | Saga | Éxito: habitación bloqueada (incluye `RoomId`, `Amount`, `Currency`) |
| `RoomReservationFailed` | Inventory | Saga | Falla: no había habitación disponible |
| `ProcessPayment` | Saga | Payments | Comando: pide cobrar el importe |
| `PaymentCompleted` | Payments | Saga | Éxito: pago aprobado |
| `PaymentFailed` | Payments | Saga | Falla: el cobro no se pudo procesar |
| `ReleaseRoom` | Saga | Inventory | Comando de **compensación**: libera la habitación bloqueada |
| `ReservationConfirmed` | Saga | Reservations | Resultado final feliz → `Reservation.Confirm()` |
| `ReservationRejected` | Saga | Reservations | Resultado final (sin habitación) → `Reservation.Reject()` |
| `ReservationCancelled` | Saga | Reservations | Resultado final (compensación por pago fallido) → `Reservation.Cancel()` |

`Amount`/`Currency` de `RoomReserved` se calculan en Inventory (`PricePerNight × noches`), porque
sólo ese servicio conoce el precio de la habitación; la saga simplemente los reenvía a
`ProcessPayment`.

## Componentes desarrollados

### `Hotel.Shared`
- 11 contratos de mensaje (records POCO, sin dependencia a MassTransit) en `ServiceBusMessages/`.

### `Hotel.Reservations`
- `Application/Commands`: `CreateReservation` (ahora publica `ReservationRequested`),
  `ConfirmReservation`, `RejectReservation`, `CancelReservation` (invocados por los consumers de
  resultado final), `UpdateReservation`, `DeleteReservation`.
- `Api/Sagas/ReservationStateMachine` + `ReservationState`: la saga orquestadora — estados
  `AwaitingRoomReservation` → `AwaitingPayment` → final (`Confirmed`/`Rejected`/`Cancelled`).
- `Api/Consumers`: `ReservationConfirmedConsumer`, `ReservationRejectedConsumer`,
  `ReservationCancelledConsumer` — aplican el resultado de la saga sobre el agregado `Reservation`
  vía MediatR.

### `Hotel.Inventory`
- `Application/Commands`: `ReserveRoom` (bloquea una habitación libre y calcula el importe),
  `ReleaseRoom` (compensación, idempotente), además de `CreateRoom`/`UpdateRoom`/`DeleteRoom`/
  `Discontinue`/`Reactivate` ya existentes.
- `Api/Consumers`: `CheckRoomAvailabilityConsumer`, `ReleaseRoomConsumer`.
- `IRoomRepository.GetByReservationIdAsync`: nuevo método para ubicar la habitación bloqueada por
  una reserva a partir sólo del `ReservationId` (lo que trae el mensaje de compensación).

### `Hotel.Payments`
- `Api/Consumers/ProcessPaymentConsumer`: crea el pago, lo aprueba (no hay pasarela real) y
  publica `PaymentCompleted`; si algo falla, publica `PaymentFailed` para que la saga compense.

### Prerrequisitos que no existían y se completaron
`Hotel.Inventory.Api` y `Hotel.Payments.Api` tenían el `Program.cs` de plantilla en blanco (sin
`AddMediatR`, `AddDbContext`, repositorios ni MassTransit registrados, pese a que ese código ya
existía). Se completó el *composition root* de ambos siguiendo el mismo patrón que ya tenía
Reservations.Api.

## Compilación

Requiere **.NET SDK 10**. Hay una solución (`.slnx`) por servicio — no hay un `.sln` único en la
raíz que los una a todos:

```powershell
dotnet build src\Shared\Hotel.Shared\Hotel.Shared.slnx
dotnet build src\Service\Hotel.Reservations\Hotel.Reservations.slnx
dotnet build src\Service\Hotel.Inventory\Hotel.Inventory.slnx
dotnet build src\Service\Hotel.Payments\Hotel.Payments.slnx
```

## Configuración

Cada servicio expone su cadena de conexión y la config de RabbitMQ en
`src/.../Api/appsettings.Development.json`. Por defecto, los 3 apuntan a:

```json
"Database": { "ConnectionString": "Server=.;Database=Hotel_<Servicio>;Trusted_Connection=True;TrustServerCertificate=True" },
"RabbitMq": { "Host": "localhost", "Port": 5672, "Username": "guest", "Password": "guest", "VirtualHost": "/" }
```

Ajustá `Database.ConnectionString` si tu SQL Server no es una instancia local por defecto
(`Server=.`). En `Development`, cada Api llama `EnsureCreated()` al arrancar, así que no hace
falta correr migraciones a mano.

### Infraestructura necesaria

- **SQL Server** accesible con la cadena de conexión de arriba (una base por servicio:
  `Hotel_Reservations`, `Hotel_Inventory`, `Hotel_Payment`).
- **RabbitMQ** en `localhost:5672` (consola de administración en `15672`). Con Docker:
  ```powershell
  docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
  ```

## Ejecución

Arrancá los 3 Api (cada uno en su propia terminal, o con `--launch-profile https`):

```powershell
dotnet run --project src\Service\Hotel.Reservations\src\Hotel.Reservations.Api   # https://localhost:7060
dotnet run --project src\Service\Hotel.Inventory\src\Hotel.Inventory.Api         # https://localhost:7237
dotnet run --project src\Service\Hotel.Payments\src\Hotel.Payments.Api          # https://localhost:7291
```

Cada uno expone Swagger en `/swagger` cuando corre en `Development`.

### Probar el flujo feliz (reserva confirmada)

1. Crear una habitación disponible:
   ```
   POST https://localhost:7237/api/Rooms
   { "hotelId": "H1", "roomType": "Doble", "roomNumber": "101", "pricePerNight": 150 }
   ```
2. Crear la reserva (esto dispara la saga):
   ``` https://localhost:7060/swagger/index.html
   POST https://localhost:7060/api/Reservation
   {
     "customerId": "C1", "hotelId": "H1", "roomType": "Doble",
     "checkIn": "2026-08-01", "checkOut": "2026-08-03", "guests": 2
   }
   ```
3. Consultar la reserva unos segundos después — debería terminar en `Confirmed`:
   ```
   GET https://localhost:7060/api/Reservation/{id}
   ```
   De paso, `GET https://localhost:7291/api/Payments/by-reservation/{id}` muestra el pago
   `Completed` que generó la saga, y `GET https://localhost:7237/api/Rooms/{roomId}` muestra la
   habitación como reservada.

### Probar el rechazo (sin habitación disponible)

Repetí el paso 2 con un `hotelId`/`roomType` para el que no creaste ninguna habitación — la
reserva debería terminar en `Rejected` sin que se genere ningún pago.

### Ver las colas y mensajes

Con RabbitMQ levantado con el plugin de administración, `http://localhost:15672` (usuario/clave
`guest`/`guest`) muestra las colas que `ConfigureEndpoints` crea automáticamente para cada
consumer y la saga.


reserva : http://localhost:5052/swagger/index.html