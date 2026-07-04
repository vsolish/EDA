using Hotel.Reservations.Application.Commands.RejectReservation;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Reservations.Api.Consumers;

public class ReservationRejectedConsumer : IConsumer<ReservationRejected>
{
    private readonly IMediator _mediator;

    public ReservationRejectedConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<ReservationRejected> context)
    {
        await _mediator.Send(new RejectReservationCommand(context.Message.ReservationId, context.Message.Reason));
    }
}
