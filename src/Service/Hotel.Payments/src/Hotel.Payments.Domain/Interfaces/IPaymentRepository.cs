using Hotel.Payments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca el pago asociado a una reserva. Se usa, por ejemplo, cuando
    /// el SAGA necesita reembolsar el cobro de una reserva que se cancela.
    /// </summary>
    Task<Payment?> GetByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
