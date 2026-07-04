using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Commands.DeleteRoom;

public record DeleteRoomCommand(Guid Id) : IRequest<bool>;