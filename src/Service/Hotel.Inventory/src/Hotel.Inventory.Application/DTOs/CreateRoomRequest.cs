using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.DTOs;

public class CreateRoomRequest
{
    public string HotelId { get; set; } = default!;
    public string RoomType { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public decimal PricePerNight { get; set; }
}
