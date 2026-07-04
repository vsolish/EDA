
using Hotel.Inventory.Application.DTOs;
using MediatR;

namespace Hotel.Inventory.Application.Commands.DiscontinueRoom;

public record DiscontinueRoomCommand(Guid Id) : IRequest<RoomDto>;
