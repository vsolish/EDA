using Hotel.Payments.Application.Exceptions;
using Hotel.Payments.Domain.Enums;
using Hotel.Payments.Domain.Interfaces;
using MediatR;


namespace Hotel.Payments.Application.Commands.DeletePayment;

public class DeletePaymentHandler : IRequestHandler<DeletePaymentCommand, bool>
{
    private readonly IPaymentRepository _repository;

    public DeletePaymentHandler(IPaymentRepository repository) => _repository = repository;

    public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Payment with ID {request.Id} not found");

        // No se borra un pago que ya fue aprobado: eliminarlo dejaría un
        // cobro real sin rastro contable. Sólo se permite limpiar intentos
        // que quedaron Pending o que fueron Failed/Refunded.
        if (payment.Status == PaymentStatus.Completed)
        {
            throw new InvalidOperationException(
                $"No se puede eliminar el pago {payment.Id}: está en estado Completed. Use RefundPaymentCommand si corresponde revertirlo.");
        }

        await _repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}

