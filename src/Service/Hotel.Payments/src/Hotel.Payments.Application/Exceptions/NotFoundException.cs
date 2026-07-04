using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

