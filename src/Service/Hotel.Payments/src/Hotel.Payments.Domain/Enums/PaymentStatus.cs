using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Domain.Enums;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}