using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.DTOs;

// HotelId no es editable: cambiar de hotel a una habitación física ya
// existente no tiene sentido de negocio; se modela como Discontinue()
// de la habitación vieja + creación de una nueva en el hotel correcto.
public class UpdateRoomRequest
{
    public string RoomType { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public decimal PricePerNight { get; set; }
}