namespace Hotel.Shared.ServiceBusMessages;

public record RoomReserved(
    Guid ReservationId,
    Guid RoomId,
    decimal Amount,
    string Currency);
