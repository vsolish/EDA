using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Domain.Entities;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Commands.CreateRoom;

public class CreateRoomHandler : IRequestHandler<CreateRoomCommand, RoomDto>
{
    private readonly IRoomRepository _repository;

    public CreateRoomHandler(IRoomRepository repository) => _repository = repository;

    public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        // El constructor de Room valida sus propias invariantes (campos
        // obligatorios, PricePerNight > 0) y arranca en RoomStatus.Active
        // con Availability.Free() 
        var room = new Room(
            request.Request.HotelId,
            request.Request.RoomType,
            request.Request.RoomNumber,
            request.Request.PricePerNight
        );

        await _repository.AddAsync(room, cancellationToken);

        return room.Adapt<RoomDto>();
    }
}