using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

