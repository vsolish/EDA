using Hotel.Payments.Application.Commands.CompletePayment;
using Hotel.Payments.Application.Commands.CreatePayment;
using Hotel.Payments.Application.Commands.DeletePayment;
using Hotel.Payments.Application.Commands.FailPayment;
using Hotel.Payments.Application.Commands.RefundPayment;
using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Application.Queries.GetAllPayments;
using Hotel.Payments.Application.Queries.GetPaymentById;
using Hotel.Payments.Application.Queries.GetPaymentByReservationId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Payments.Api.Controllers;
/// <summary>
/// controlador no recibe un
/// orquestador de SAGA: en esta arquitectura el SAGA vive en
/// Reservations, no en Payments  es un
/// PARTICIPANTE del SAGA, no quien lo inicia — el camino normal de
/// creación de un pago es automático, vía ProcessPaymentConsumer
/// reaccionando al comando ProcessPayment que publica el SAGA. El
/// POST de este controlador es un camino administrativo/manual aparte,
/// que no dispara ningún proceso distribuido.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetAll(CancellationToken cancellationToken)
       => Ok(await mediator.Send(new GetAllPaymentsQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-reservation/{reservationId:guid}")]
    public async Task<ActionResult<PaymentDto>> GetByReservationId(Guid reservationId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentByReservationIdQuery(reservationId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Creación administrativa/manual (Pending). No dispara ningún SAGA.</summary>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreatePaymentCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletePaymentCommand(id), cancellationToken);
        return NoContent();
    }

    // No hay un único PATCH /{id}/status como en OrdersController: a
    // diferencia de un status genérico, cada transición de Payment tiene
    // su propio contrato (Fail exige Reason, Complete/Refund no reciben
    // body), así que se exponen como tres acciones explícitas — más
    // type-safe y cada una con su propio validador de FluentValidation.

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<PaymentDto>> Complete(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new CompletePaymentCommand(id), cancellationToken));

    [HttpPost("{id:guid}/fail")]
    public async Task<ActionResult<PaymentDto>> Fail(Guid id, [FromBody] FailPaymentRequest request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new FailPaymentCommand(id, request), cancellationToken));

    [HttpPost("{id:guid}/refund")]
    public async Task<ActionResult<PaymentDto>> Refund(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new RefundPaymentCommand(id), cancellationToken));
}
