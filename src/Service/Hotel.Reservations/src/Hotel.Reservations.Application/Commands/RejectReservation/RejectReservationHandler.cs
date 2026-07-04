using Hotel.Reservations.Application.DTOs;
using Hotel.Reservations.Application.Exceptions;
using Hotel.Reservations.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Reservations.Application.Commands.RejectReservation;

/// <summary>
/// Se invoca desde ReservationRejectedConsumer cuando el SAGA no encontró
/// habitación disponible. No hay pago que compensar porque nunca se cobró.
/// </summary>
public class RejectReservationHandler : IRequestHandler<RejectReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _repository;

    public RejectReservationHandler(IReservationRepository repository) => _repository = repository;

    public async Task<ReservationDto> Handle(RejectReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Reservation with ID {request.Id} not found");

        reservation.Reject(request.Reason);
        await _repository.UpdateAsync(reservation, cancellationToken);

        return reservation.Adapt<ReservationDto>();
    }
}
