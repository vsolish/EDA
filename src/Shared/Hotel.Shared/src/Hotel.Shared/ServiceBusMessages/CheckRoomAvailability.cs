namespace Hotel.Shared.ServiceBusMessages;

public record CheckRoomAvailability(
    Guid ReservationId,
    string HotelId,
    string RoomType,
    DateTime CheckIn,
    DateTime CheckOut);
