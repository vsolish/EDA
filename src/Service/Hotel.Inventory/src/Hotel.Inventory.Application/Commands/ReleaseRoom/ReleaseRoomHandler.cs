using Hotel.Inventory.Domain.Interfaces;
using MediatR;

namespace Hotel.Inventory.Application.Commands.ReleaseRoom;

/// <summary>
/// Compensación SAGA: libera la habitación bloqueada por una reserva que
/// terminó cancelándose (p. ej. porque falló el cobro). Si no se encuentra
/// ninguna habitación bloqueada por ese ReservationId, no hace nada — es
/// idempotente ante mensajes duplicados o reservas que nunca llegaron a
/// bloquear una habitación.
/// </summary>
public class ReleaseRoomHandler : IRequestHandler<ReleaseRoomCommand>
{
    private readonly IRoomRepository _repository;

    public ReleaseRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task Handle(ReleaseRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByReservationIdAsync(request.ReservationId, cancellationToken);
        if (room is null)
            return;

        room.Release(request.ReservationId);
        await _repository.UpdateAsync(room, cancellationToken);
    }
}
