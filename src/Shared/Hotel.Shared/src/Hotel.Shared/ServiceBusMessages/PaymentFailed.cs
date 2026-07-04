namespace Hotel.Shared.ServiceBusMessages;

public record PaymentFailed(
    Guid ReservationId,
    string Reason);
