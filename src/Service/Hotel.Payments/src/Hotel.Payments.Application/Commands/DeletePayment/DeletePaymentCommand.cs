using MediatR;

namespace Hotel.Payments.Application.Commands.DeletePayment;

public record DeletePaymentCommand(Guid Id) : IRequest<bool>;
