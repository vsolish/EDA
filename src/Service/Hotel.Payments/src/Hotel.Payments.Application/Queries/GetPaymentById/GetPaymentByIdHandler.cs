using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;


namespace Hotel.Payments.Application.Queries.GetPaymentById;

public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByIdHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return payment?.Adapt<PaymentDto>();
    }
}
