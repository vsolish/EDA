using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Application.Exceptions;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Commands.CompletePayment;

public class CompletePaymentHandler : IRequestHandler<CompletePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _repository;

    public CompletePaymentHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Payment with ID {request.Id} not found");

        payment.Complete();
        await _repository.UpdateAsync(payment, cancellationToken);

        return payment.Adapt<PaymentDto>();
    }
}