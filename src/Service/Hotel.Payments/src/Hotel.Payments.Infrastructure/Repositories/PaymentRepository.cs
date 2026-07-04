using Hotel.Payments.Domain.Entities;
using Hotel.Payments.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Hotel.Payments.Infrastructure.Repositories;


public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Payment?> GetByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
        => await context.Payments.FirstOrDefaultAsync(p => p.ReservationId == reservationId, cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Payments.ToListAsync(cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await context.Payments.AddAsync(payment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        context.Payments.Update(payment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await GetByIdAsync(id, cancellationToken);
        if (payment is not null)
        {
            context.Payments.Remove(payment);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}