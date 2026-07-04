using Hotel.Payments.Application.DTOs;
using MediatR;


namespace Hotel.Payments.Application.Commands.FailPayment;

public record FailPaymentCommand(Guid Id, FailPaymentRequest Request) : IRequest<PaymentDto>;
