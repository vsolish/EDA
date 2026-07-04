
using Hotel.Inventory.Application.DTOs;
using MediatR;

namespace Hotel.Inventory.Application.Commands.UpdateRoom;

public record UpdateRoomCommand(Guid Id, UpdateRoomRequest Request) : IRequest<RoomDto>;
