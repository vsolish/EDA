using Hotel.Payments.Application.DTOs;
using MediatR;


namespace Hotel.Payments.Application.Queries.GetAllPayments;

public record GetAllPaymentsQuery : IRequest<IReadOnlyList<PaymentDto>>;
