using Hotel.Reservations.Application.Commands.CancelReservation;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Reservations.Api.Consumers;

public class ReservationCancelledConsumer : IConsumer<ReservationCancelled>
{
    private readonly IMediator _mediator;

    public ReservationCancelledConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<ReservationCancelled> context)
    {
        await _mediator.Send(new CancelReservationCommand(context.Message.ReservationId, context.Message.Reason));
    }
}
