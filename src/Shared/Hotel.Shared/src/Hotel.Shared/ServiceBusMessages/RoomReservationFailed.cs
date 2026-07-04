namespace Hotel.Shared.ServiceBusMessages;

public record RoomReservationFailed(
    Guid ReservationId,
    string Reason);
