using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Reservations.Application.DTOs;

public record ReservationDto(
    Guid Id,
    string CustomerId,
    string HotelId,
    string RoomType,
    DateTime CheckIn,
    DateTime CheckOut,
    int Nights,
    int Guests,
    string Status,
    string? StatusReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
