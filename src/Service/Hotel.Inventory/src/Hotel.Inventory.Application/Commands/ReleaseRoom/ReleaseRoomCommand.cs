using MediatR;

namespace Hotel.Inventory.Application.Commands.ReleaseRoom;

public record ReleaseRoomCommand(Guid ReservationId) : IRequest;
