using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Domain.Entities;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Commands.CreatePayment;

/// <summary>
/// Creación administrativa/manual de un pago (Pending). El camino normal
/// del sistema crea pagos automáticamente vía ProcessPaymentConsumer al
/// reaccionar al comando ProcessPayment del SAGA; este comando existe
/// para flujos administrativos o pruebas fuera de ese camino.
/// </summary>
public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _repository;

    public CreatePaymentHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(
            request.Request.ReservationId,
            request.Request.CustomerId,
            request.Request.Amount,
            request.Request.Currency
        );

        await _repository.AddAsync(payment, cancellationToken);

        return payment.Adapt<PaymentDto>();
    }
}
