using Hotel.Reservations.Application.DTOs;
using Hotel.Reservations.Application.Exceptions;
using Hotel.Reservations.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Reservations.Application.Commands.ConfirmReservation;

/// <summary>
/// Se invoca desde ReservationConfirmedConsumer cuando el SAGA completó el
/// bloqueo de habitación y el cobro.
/// </summary>
public class ConfirmReservationHandler : IRequestHandler<ConfirmReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _repository;

    public ConfirmReservationHandler(IReservationRepository repository) => _repository = repository;

    public async Task<ReservationDto> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Reservation with ID {request.Id} not found");

        reservation.Confirm();
        await _repository.UpdateAsync(reservation, cancellationToken);

        return reservation.Adapt<ReservationDto>();
    }
}
