using Hotel.Reservations.Application.Commands.ConfirmReservation;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Reservations.Api.Consumers;

public class ReservationConfirmedConsumer : IConsumer<ReservationConfirmed>
{
    private readonly IMediator _mediator;

    public ReservationConfirmedConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<ReservationConfirmed> context)
    {
        await _mediator.Send(new ConfirmReservationCommand(context.Message.ReservationId));
    }
}
