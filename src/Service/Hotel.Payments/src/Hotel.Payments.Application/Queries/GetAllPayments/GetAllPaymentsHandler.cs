using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;



namespace Hotel.Payments.Application.Queries.GetAllPayments;

public class GetAllPaymentsHandler : IRequestHandler<GetAllPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IPaymentRepository _repository;

    public GetAllPaymentsHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _repository.GetAllAsync(cancellationToken);
        return payments.Adapt<List<PaymentDto>>();
    }
}
