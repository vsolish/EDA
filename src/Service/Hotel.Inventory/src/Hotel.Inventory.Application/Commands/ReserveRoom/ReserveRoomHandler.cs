using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Domain.Interfaces;
using MediatR;

namespace Hotel.Inventory.Application.Commands.ReserveRoom;

/// <summary>
/// Reacciona al comando CheckRoomAvailability que publica el SAGA:
/// busca una habitación libre del hotel/tipo pedidos y, si existe, la
/// bloquea para la reserva (equivalente a ReserveStock).
/// </summary>
public class ReserveRoomHandler : IRequestHandler<ReserveRoomCommand, ReserveRoomResult>
{
    private readonly IRoomRepository _repository;

    public ReserveRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<ReserveRoomResult> Handle(ReserveRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.FindAvailableRoomAsync(request.HotelId, request.RoomType, cancellationToken);
        if (room is null)
        {
            return new ReserveRoomResult(false, null, 0, string.Empty,
                $"No hay habitaciones disponibles del tipo '{request.RoomType}' en el hotel '{request.HotelId}'.");
        }

        room.Reserve(request.ReservationId);
        await _repository.UpdateAsync(room, cancellationToken);

        var nights = Math.Max(1, (request.CheckOut.Date - request.CheckIn.Date).Days);
        var amount = room.PricePerNight * nights;

        return new ReserveRoomResult(true, room.Id, amount, "PEN", null);
    }
}
