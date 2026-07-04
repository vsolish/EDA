namespace Hotel.Shared.ServiceBusMessages;

public record ReservationCancelled(
    Guid ReservationId,
    string Reason);
