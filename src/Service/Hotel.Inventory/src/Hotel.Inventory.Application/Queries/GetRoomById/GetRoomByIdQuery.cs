using Hotel.Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Queries.GetRoomById;


public record GetRoomByIdQuery(Guid Id) : IRequest<RoomDto?>;