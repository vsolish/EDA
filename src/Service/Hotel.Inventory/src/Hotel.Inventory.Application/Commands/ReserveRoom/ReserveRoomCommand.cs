using Hotel.Inventory.Application.DTOs;
using MediatR;

namespace Hotel.Inventory.Application.Commands.ReserveRoom;

public record ReserveRoomCommand(
    Guid ReservationId,
    string HotelId,
    string RoomType,
    DateTime CheckIn,
    DateTime CheckOut) : IRequest<ReserveRoomResult>;
