using Hotel.Shared.ServiceBusMessages;
using MassTransit;

namespace Hotel.Reservations.Api.Sagas;

/// <summary>
/// Orquesta la creación de una reserva: bloquea habitación (Inventory),
/// cobra (Payments) y confirma la reserva, o compensa (libera habitación /
/// rechaza / cancela) si alguno de los dos pasos falla.
/// </summary>
public class ReservationStateMachine : MassTransitStateMachine<ReservationState>
{
    public State AwaitingRoomReservation { get; private set; } = null!;
    public State AwaitingPayment { get; private set; } = null!;

    public Event<ReservationRequested> ReservationRequested { get; private set; } = null!;
    public Event<RoomReserved> RoomReserved { get; private set; } = null!;
    public Event<RoomReservationFailed> RoomReservationFailed { get; private set; } = null!;
    public Event<PaymentCompleted> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;

    public ReservationStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ReservationRequested, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => RoomReserved, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => RoomReservationFailed, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => PaymentCompleted, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => PaymentFailed, e => e.CorrelateById(m => m.Message.ReservationId));

        Initially(
            When(ReservationRequested)
                .Then(context =>
                {
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.HotelId = context.Message.HotelId;
                    context.Saga.RoomType = context.Message.RoomType;
                    context.Saga.CheckIn = context.Message.CheckIn;
                    context.Saga.CheckOut = context.Message.CheckOut;
                    context.Saga.Guests = context.Message.Guests;
                })
                .Publish(context => new CheckRoomAvailability(
                    context.Saga.CorrelationId,
                    context.Saga.HotelId,
                    context.Saga.RoomType,
                    context.Saga.CheckIn,
                    context.Saga.CheckOut))
                .TransitionTo(AwaitingRoomReservation));

        During(AwaitingRoomReservation,
            When(RoomReserved)
                .Then(context =>
                {
                    context.Saga.RoomId = context.Message.RoomId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Currency = context.Message.Currency;
                })
                .Publish(context => new ProcessPayment(
                    context.Saga.CorrelationId,
                    context.Saga.CustomerId,
                    context.Saga.Amount,
                    context.Saga.Currency))
                .TransitionTo(AwaitingPayment),
            When(RoomReservationFailed)
                .Then(context => context.Saga.FailureReason = context.Message.Reason)
                .Publish(context => new ReservationRejected(context.Saga.CorrelationId, context.Saga.FailureReason!))
                .Finalize());

        During(AwaitingPayment,
            When(PaymentCompleted)
                .Then(context => context.Saga.PaymentId = context.Message.PaymentId)
                .Publish(context => new ReservationConfirmed(context.Saga.CorrelationId))
                .Finalize(),
            When(PaymentFailed)
                .Then(context => context.Saga.FailureReason = context.Message.Reason)
                .Publish(context => new ReleaseRoom(context.Saga.CorrelationId))
                .Publish(context => new ReservationCancelled(context.Saga.CorrelationId, context.Saga.FailureReason!))
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
