using Hotel.Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Queries.GetAllRooms;

public record GetAllRoomsQuery : IRequest<IReadOnlyList<RoomDto>>;