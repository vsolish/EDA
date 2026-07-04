using Hotel.Payments.Domain.Entities;
using Hotel.Payments.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Payments.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentConfiguration).Assembly);
    }
}