using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.DTOs;

public record RoomDto(
    Guid Id,
    string HotelId,
    string RoomType,
    string RoomNumber,
    decimal PricePerNight,
    bool IsReserved,
    Guid? ReservationId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);