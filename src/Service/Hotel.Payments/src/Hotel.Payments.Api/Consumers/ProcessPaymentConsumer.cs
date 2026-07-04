using Hotel.Payments.Application.Commands.CompletePayment;
using Hotel.Payments.Application.Commands.CreatePayment;
using Hotel.Payments.Application.DTOs;
using Hotel.Shared.ServiceBusMessages;
using MassTransit;
using MediatR;

namespace Hotel.Payments.Api.Consumers;

/// <summary>
/// Reacciona al comando ProcessPayment que publica el SAGA: crea el pago
/// (Pending) y lo aprueba (no hay pasarela real, siempre se completa). Si
/// cualquiera de los dos pasos falla, publica PaymentFailed para que el
/// SAGA compense (libera la habitación y cancela la reserva).
/// </summary>
public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
{
    private readonly IMediator _mediator;

    public ProcessPaymentConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        try
        {
            var payment = await _mediator.Send(new CreatePaymentCommand(new CreatePaymentRequest
            {
                ReservationId = context.Message.ReservationId,
                CustomerId = context.Message.CustomerId,
                Amount = context.Message.Amount,
                Currency = context.Message.Currency
            }));

            payment = await _mediator.Send(new CompletePaymentCommand(payment.Id));

            await context.Publish(new PaymentCompleted(context.Message.ReservationId, payment.Id));
        }
        catch (Exception ex)
        {
            await context.Publish(new PaymentFailed(context.Message.ReservationId, ex.Message));
        }
    }
}
