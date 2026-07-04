using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Domain.Interfaces;
using Mapster;
using MediatR;


namespace Hotel.Payments.Application.Queries.GetPaymentByReservationId;

public class GetPaymentByReservationIdHandler : IRequestHandler<GetPaymentByReservationIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _repository;

    public GetPaymentByReservationIdHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<PaymentDto?> Handle(GetPaymentByReservationIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByReservationIdAsync(request.ReservationId, cancellationToken);
        return payment?.Adapt<PaymentDto>();
    }
}
