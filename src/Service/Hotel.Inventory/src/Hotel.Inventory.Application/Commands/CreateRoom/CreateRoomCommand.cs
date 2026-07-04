using Hotel.Inventory.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Commands.CreateRoom;

public record CreateRoomCommand(CreateRoomRequest Request) : IRequest<RoomDto>;
