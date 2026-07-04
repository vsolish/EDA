using Hotel.Payments.Domain.Enums;
using Hotel.Payments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid ReservationId { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public Money Money { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public string? StatusReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Payment() { }

    public Payment(Guid reservationId, string customerId, decimal amount, string currency = "PEN")
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("El ReservationId es obligatorio.", nameof(reservationId));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("El CustomerId es obligatorio.", nameof(customerId));

        Id = Guid.NewGuid();
        ReservationId = reservationId;
        CustomerId = customerId;
        Money = new Money(amount, currency); // valida Amount > 0 y Currency ISO 4217
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>La pasarela de pago aprobó el cargo.</summary>
    public void Complete()
    {
        EnsureIsPending(nameof(Complete));

        Status = PaymentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>La pasarela de pago rechazó el cargo.</summary>
    public void Fail(string reason)
    {
        EnsureIsPending(nameof(Fail));

        Status = PaymentStatus.Failed;
        StatusReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reembolsa un pago ya aprobado (compensación SAGA cuando la reserva
    /// se cancela después de haber sido cobrada). Sólo procede desde
    /// Completed: un pago que nunca se aprobó no tiene nada que reembolsar.
    /// </summary>
    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
        {
            throw new InvalidOperationException(
                $"Sólo se puede reembolsar un pago en estado '{PaymentStatus.Completed}'; el estado actual es '{Status}'.");
        }

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureIsPending(string operation)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                $"No se puede ejecutar '{operation}' sobre el pago {Id}: su estado actual es '{Status}', se esperaba '{PaymentStatus.Pending}'.");
        }
    }
}

