using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.DTOs;

public class CreatePaymentRequest
{
    public Guid ReservationId { get; set; }
    public string CustomerId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "PEN";
}
