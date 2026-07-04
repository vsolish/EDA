using Hotel.Inventory.Application.Exceptions;
using Hotel.Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Commands.DeleteRoom;


public class DeleteRoomHandler : IRequestHandler<DeleteRoomCommand, bool>
{
    private readonly IRoomRepository _repository;

    public DeleteRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<bool> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Room with ID {request.Id} not found");

        // no se puede eliminar una habitación mientras Availability.IsReserved está en
        // true, porque rompería la trazabilidad de la reserva activa que
        // la está bloqueando (el SAGA aún podría publicar ReleaseRoom
        // sobre un RoomId que ya no existiría).
        if (room.Availability.IsReserved)
        {
            throw new InvalidOperationException(
                $"No se puede eliminar la habitación {room.RoomNumber}: está bloqueada por la reserva {room.Availability.ReservationId}.");
        }

        await _repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}