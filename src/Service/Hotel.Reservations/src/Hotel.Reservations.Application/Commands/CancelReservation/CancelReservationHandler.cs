using Hotel.Reservations.Application.DTOs;
using Hotel.Reservations.Application.Exceptions;
using Hotel.Reservations.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Reservations.Application.Commands.CancelReservation;

/// <summary>
/// Se invoca desde ReservationCancelledConsumer cuando el SAGA falló al
/// cobrar el pago; es la compensación automática (la habitación ya fue
/// liberada por el SAGA vía ReleaseRoom antes de llegar a este punto).
/// </summary>
public class CancelReservationHandler : IRequestHandler<CancelReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _repository;

    public CancelReservationHandler(IReservationRepository repository) => _repository = repository;

    public async Task<ReservationDto> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Reservation with ID {request.Id} not found");

        reservation.Cancel(request.Reason);
        await _repository.UpdateAsync(reservation, cancellationToken);

        return reservation.Adapt<ReservationDto>();
    }
}
