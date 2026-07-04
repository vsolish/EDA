namespace Hotel.Shared.ServiceBusMessages;

public record PaymentCompleted(
    Guid ReservationId,
    Guid PaymentId);
