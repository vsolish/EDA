using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Application.Exceptions;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Queries.GetRoomById;

public class GetRoomByIdHandler : IRequestHandler<GetRoomByIdQuery, RoomDto?>
{
    private readonly IRoomRepository _repository;

    public GetRoomByIdHandler(IRoomRepository repository) => _repository = repository;

    public async Task<RoomDto?> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            throw new NotFoundException($"Room {request.Id} not found");
        return product.Adapt<RoomDto>();
    }
}