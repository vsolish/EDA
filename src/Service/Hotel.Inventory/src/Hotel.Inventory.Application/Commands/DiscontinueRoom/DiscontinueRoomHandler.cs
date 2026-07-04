
using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Application.Exceptions;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Inventory.Application.Commands.DiscontinueRoom;

/// <summary>
/// Comando administrativo: saca una habitación de servicio (por ejemplo,
/// para mantenimiento). Una vez Discontinued, Room.Reserve(...) la
/// rechaza automáticamente (el agregado valida Status == Active antes de
/// bloquear), por lo que dejará de aparecer como candidata en
/// FindAvailableRoomAsync sin necesidad de borrarla.
/// </summary>
public class DiscontinueRoomHandler : IRequestHandler<DiscontinueRoomCommand, RoomDto>
{
    private readonly IRoomRepository _repository;

    public DiscontinueRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<RoomDto> Handle(DiscontinueRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Room with ID {request.Id} not found");

        room.Discontinue();
        await _repository.UpdateAsync(room, cancellationToken);

        return room.Adapt<RoomDto>();
    }
}
