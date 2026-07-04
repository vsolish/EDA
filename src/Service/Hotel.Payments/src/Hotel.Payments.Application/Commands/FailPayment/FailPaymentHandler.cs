using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Application.Exceptions;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;


namespace Hotel.Payments.Application.Commands.FailPayment;

public class FailPaymentHandler : IRequestHandler<FailPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _repository;

    public FailPaymentHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto> Handle(FailPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Payment with ID {request.Id} not found");

        payment.Fail(request.Request.Reason);
        await _repository.UpdateAsync(payment, cancellationToken);

        return payment.Adapt<PaymentDto>();
    }
}
