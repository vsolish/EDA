using Hotel.Payments.Application.DTOs;
using MediatR;


namespace Hotel.Payments.Application.Commands.RefundPayment;

public record RefundPaymentCommand(Guid Id) : IRequest<PaymentDto>;
