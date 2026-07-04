using MassTransit;

namespace Hotel.Reservations.Api.Sagas;

public class ReservationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;

    public string CustomerId { get; set; } = default!;
    public string HotelId { get; set; } = default!;
    public string RoomType { get; set; } = default!;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Guests { get; set; }

    public Guid? RoomId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "PEN";
    public Guid? PaymentId { get; set; }

    public string? FailureReason { get; set; }
}
