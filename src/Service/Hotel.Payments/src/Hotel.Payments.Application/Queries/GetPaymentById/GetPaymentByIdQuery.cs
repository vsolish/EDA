using Hotel.Payments.Application.DTOs;
using MediatR;


namespace Hotel.Payments.Application.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto?>;
