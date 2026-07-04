using Hotel.Inventory.Application.Commands.ReserveRoom;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Inventory.Api.Consumers;

public class CheckRoomAvailabilityConsumer : IConsumer<CheckRoomAvailability>
{
    private readonly IMediator _mediator;

    public CheckRoomAvailabilityConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<CheckRoomAvailability> context)
    {
        var result = await _mediator.Send(new ReserveRoomCommand(
            context.Message.ReservationId,
            context.Message.HotelId,
            context.Message.RoomType,
            context.Message.CheckIn,
            context.Message.CheckOut));

        if (result.Success)
        {
            await context.Publish(new RoomReserved(
                context.Message.ReservationId,
                result.RoomId!.Value,
                result.Amount,
                result.Currency));
        }
        else
        {
            await context.Publish(new RoomReservationFailed(
                context.Message.ReservationId,
                result.FailureReason!));
        }
    }
}
