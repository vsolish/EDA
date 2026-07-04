namespace Hotel.Shared.ServiceBusMessages;

public record ReservationRequested(
    Guid ReservationId,
    string CustomerId,
    string HotelId,
    string RoomType,
    DateTime CheckIn,
    DateTime CheckOut,
    int Guests,
    DateTime RequestedAt);
