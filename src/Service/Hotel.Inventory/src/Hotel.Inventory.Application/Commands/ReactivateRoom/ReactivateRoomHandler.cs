using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Application.Exceptions;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Inventory.Application.Commands.ReactivateRoom;

/// <summary>
/// Comando administrativo inverso a DiscontinueRoom: vuelve a poner la
/// habitación en servicio (Status = Active), permitiendo que vuelva a
/// aparecer como candidata en las búsquedas de disponibilidad.
/// </summary>
public class ReactivateRoomHandler : IRequestHandler<ReactivateRoomCommand, RoomDto>
{
    private readonly IRoomRepository _repository;

    public ReactivateRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<RoomDto> Handle(ReactivateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Room with ID {request.Id} not found");

        room.Reactivate();
        await _repository.UpdateAsync(room, cancellationToken);

        return room.Adapt<RoomDto>();
    }
}
