using Hotel.Inventory.Application.Commands.ReleaseRoom;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Inventory.Api.Consumers;

public class ReleaseRoomConsumer : IConsumer<ReleaseRoom>
{
    private readonly IMediator _mediator;

    public ReleaseRoomConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<ReleaseRoom> context)
    {
        await _mediator.Send(new ReleaseRoomCommand(context.Message.ReservationId));
    }
}
