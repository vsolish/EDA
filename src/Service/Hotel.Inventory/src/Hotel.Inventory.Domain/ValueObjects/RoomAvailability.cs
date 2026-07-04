using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Domain.ValueObjects;
// <summary>
/// Value Object que representa el estado de bloqueo de una habitación.
/// aquí el "inventario" es binario (una habitación física
/// sólo puede estar libre o bloqueada por una reserva a la vez), pero
/// el rol es el mismo: encapsular la transición y sus invariantes para
/// que nunca exista un estado inconsistente (p. ej. reservada sin dueño,
/// o liberada por una reserva que no era la que la bloqueó).
/// </summary>
public sealed class RoomAvailability
{
    public bool IsReserved { get; }
    public Guid? ReservationId { get; }

    public RoomAvailability(bool isReserved, Guid? reservationId)
    {
        if (isReserved && reservationId is null)
        {
            throw new ArgumentException(
                "Una habitación reservada debe tener un ReservationId asociado.");
        }

        if (!isReserved && reservationId is not null)
        {
            throw new ArgumentException(
                "Una habitación libre no puede tener un ReservationId asociado.");
        }

        IsReserved = isReserved;
        ReservationId = reservationId;
    }

    public static RoomAvailability Free() => new(false, null);

    /// <summary>Bloquea la habitación para una reserva. Falla si ya estaba reservada.</summary>
    public RoomAvailability Reserve(Guid reservationId)
    {
        if (IsReserved)
        {
            throw new InvalidOperationException(
                $"La habitación ya está bloqueada por la reserva {ReservationId}.");
        }

        return new RoomAvailability(true, reservationId);
    }

    /// <summary>
    /// Libera la habitación. Falla si no estaba reservada, o si se
    /// intenta liberar con el ReservationId de otra reserva distinta a
    /// la que la bloqueó (evita que un mensaje duplicado/desordenado
    /// libere una habitación que en realidad pertenece a otra reserva).
    /// </summary>
    public RoomAvailability Release(Guid reservationId)
    {
        if (!IsReserved)
        {
            throw new InvalidOperationException(
                "La habitación no está reservada; no hay nada que liberar.");
        }

        if (ReservationId != reservationId)
        {
            throw new InvalidOperationException(
                $"La habitación está bloqueada por la reserva {ReservationId}, no por {reservationId}.");
        }

        return new RoomAvailability(false, null);
    }

    public override bool Equals(object? obj) =>
        obj is RoomAvailability other && IsReserved == other.IsReserved && ReservationId == other.ReservationId;

    public override int GetHashCode() => HashCode.Combine(IsReserved, ReservationId);
}
