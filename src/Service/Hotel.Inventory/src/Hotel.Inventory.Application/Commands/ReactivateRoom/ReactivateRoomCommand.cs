
using Hotel.Inventory.Application.DTOs;
using MediatR;

namespace Hotel.Inventory.Application.Commands.ReactivateRoom;

public record ReactivateRoomCommand(Guid Id) : IRequest<RoomDto>;
