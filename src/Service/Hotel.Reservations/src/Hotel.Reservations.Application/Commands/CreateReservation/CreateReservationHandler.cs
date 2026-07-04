using Hotel.Reservations.Application.DTOs;
using Hotel.Reservations.Domain.Entities;
using Hotel.Reservations.Domain.Interfaces;
using Hotel.Shared.ServiceBusMessages;
using Mapster;
// using MassTransit; // deshabilitado junto con el Publish de más abajo (requiere RabbitMQ corriendo)
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Reservations.Application.Commands.CreateReservation;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _repository;
    // private readonly IPublishEndpoint _publishEndpoint;

    public CreateReservationHandler(IReservationRepository repository) //, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        // _publishEndpoint = publishEndpoint;
    }

    public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // El propio agregado valida sus invariantes (StayPeriod, campos
        // obligatorios, etc.) dentro del constructor.
        var reservation = new Reservation(
            request.Request.CustomerId,
            request.Request.HotelId,
            request.Request.RoomType,
            request.Request.CheckIn,
            request.Request.CheckOut,
            request.Request.Guests);

        await _repository.AddAsync(reservation, cancellationToken);

        // A diferencia de CreateCustomerHandler, aquí es indispensable
        // publicar el evento de dominio: es lo que dispara el SAGA de
        // orquestación (Inventory.Service + Payments.Service). Sin este
        // Publish, la reserva queda creada pero no se procesa automáticamente.
        // Deshabilitado temporalmente porque requiere RabbitMQ/MassTransit
        // registrado en Program.cs (ver comentario allí) — descomentar ambos
        // a la vez para reactivar la SAGA.

        //await _publishEndpoint.Publish(new ReservationRequested(
        //    reservation.Id,
        //    reservation.CustomerId,
        //    reservation.HotelId,
        //    reservation.RoomType,
        //    reservation.StayPeriod.CheckIn,
        //    reservation.StayPeriod.CheckOut,
        //    reservation.Guests,
        //    reservation.CreatedAt), cancellationToken);

        return reservation.Adapt<ReservationDto>();
    }
}

