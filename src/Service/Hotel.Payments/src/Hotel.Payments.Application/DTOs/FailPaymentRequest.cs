using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.DTOs;

public class FailPaymentRequest
{
    public string Reason { get; set; } = default!;
}
