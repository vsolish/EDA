using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Application.Exceptions;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;


namespace Hotel.Payments.Application.Commands.RefundPayment;

public class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _repository;

    public RefundPaymentHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Payment with ID {request.Id} not found");

        payment.Refund();
        await _repository.UpdateAsync(payment, cancellationToken);

        return payment.Adapt<PaymentDto>();
    }
}
