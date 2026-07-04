using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Application.Exceptions;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;

namespace Hotel.Inventory.Application.Commands.UpdateRoom;

public class UpdateRoomHandler : IRequestHandler<UpdateRoomCommand, RoomDto>
{
    private readonly IRoomRepository _repository;

    public UpdateRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<RoomDto> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Room with ID {request.Id} not found");

        // room.UpdateInfo(...) es el método de comportamiento que ya
        // existe en la entidad : valida
        // sus propios argumentos y actualiza UpdatedAt. No toca
        // Availability ni Status — ese bloqueo/liberación sólo lo mueven
        // los consumers del SAGA (Reserve/Release), y Discontinue/
        // Reactivate quedan como comandos administrativos aparte.
        room.UpdateInfo(
            request.Request.RoomType,
            request.Request.RoomNumber,
            request.Request.PricePerNight);

        await _repository.UpdateAsync(room, cancellationToken);

        return room.Adapt<RoomDto>();
    }
}
