using Hotel.Inventory.Application.DTOs;
using Hotel.Inventory.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Queries.GetAllRooms;

public class GetAllRoomsHandler : IRequestHandler<GetAllRoomsQuery, IReadOnlyList<RoomDto>>
{
    private readonly IRoomRepository _repository;

    public GetAllRoomsHandler(IRoomRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<RoomDto>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Adapt<IReadOnlyList<RoomDto>>();
    }
}