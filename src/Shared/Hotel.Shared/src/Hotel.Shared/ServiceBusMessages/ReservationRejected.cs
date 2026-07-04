namespace Hotel.Shared.ServiceBusMessages;

public record ReservationRejected(
    Guid ReservationId,
    string Reason);
