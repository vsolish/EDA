namespace Hotel.Shared.ServiceBusMessages;

public record ProcessPayment(
    Guid ReservationId,
    string CustomerId,
    decimal Amount,
    string Currency);
