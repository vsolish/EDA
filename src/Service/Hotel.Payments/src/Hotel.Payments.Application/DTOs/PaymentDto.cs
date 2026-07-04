using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.DTOs;

public record PaymentDto(
    Guid Id,
    Guid ReservationId,
    string CustomerId,
    decimal Amount,
    string Currency,
    string Status,
    string? StatusReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

