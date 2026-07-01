using Bank.Loan.Domain.Enums;
using Bank.Loan.Domain.Events;
using Bank.Loan.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Loan.Domain.Entities;

/// <summary>
/// Aggregate Root del dominio de préstamos.
/// Encapsula toda la lógica de negocio y reglas de dominio.
/// No depende de ningún framework externo.
/// </summary>
public class Loans
{
    // ── Identidad ─────────────────────────────────────────────────────────
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }

    // ── Value Objects ─────────────────────────────────────────────────────
    public Money RequestedAmount { get; private set; }
    public Money? ApprovedAmount { get; private set; }
    public InterestRate? Rate { get; private set; }

    // ── Estado ────────────────────────────────────────────────────────────
    public LoanStatus Status { get; private set; }
    public int TermMonths { get; private set; }
    public string? RejectionReason { get; private set; }

    // ── Auditoría ─────────────────────────────────────────────────────────
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // ── Domain Events ─────────────────────────────────────────────────────
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();

    // Constructor privado — solo se crea via factory method
    private Loans() { CustomerId = string.Empty; RequestedAmount = null!; }

    // ── Factory Method ────────────────────────────────────────────────────

    /// <summary>Crea una nueva solicitud de préstamo.</summary>
    public static Loans Create(string customerId, decimal amount, int termMonths)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("El ID del cliente es requerido.", nameof(customerId));
        if (termMonths < 1 || termMonths > 360)
            throw new ArgumentException("El plazo debe estar entre 1 y 360 meses.", nameof(termMonths));

        var loan = new Loans
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            RequestedAmount = new Money(amount),
            TermMonths = termMonths,
            Status = LoanStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        loan._domainEvents.Add(new LoanCreatedDomainEvent(
            loan.Id, customerId, amount, loan.CreatedAt));

        return loan;
    }

    // ── Métodos de negocio ────────────────────────────────────────────────

    /// <summary>Pasa el préstamo a evaluación de riesgo.</summary>
    public void StartRiskEvaluation()
    {
        if (Status != LoanStatus.Pending)
            throw new InvalidOperationException($"No se puede evaluar un préstamo en estado {Status}.");

        Status = LoanStatus.UnderRiskEvaluation;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Aprueba el préstamo con monto y tasa.</summary>
    public void Approve(decimal approvedAmount, decimal interestRate)
    {
        if (Status != LoanStatus.UnderRiskEvaluation)
            throw new InvalidOperationException($"No se puede aprobar un préstamo en estado {Status}.");

        ApprovedAmount = new Money(approvedAmount);
        Rate = new InterestRate(interestRate);
        Status = LoanStatus.Approved;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new LoanApprovedDomainEvent(
            Id, approvedAmount, interestRate, UpdatedAt.Value));
    }

    /// <summary>Rechaza el préstamo con un motivo.</summary>
    public void Reject(string reason)
    {
        if (Status != LoanStatus.UnderRiskEvaluation)
            throw new InvalidOperationException($"No se puede rechazar un préstamo en estado {Status}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("El motivo de rechazo es requerido.", nameof(reason));

        RejectionReason = reason;
        Status = LoanStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new LoanRejectedDomainEvent(Id, reason, UpdatedAt.Value));
    }

    /// <summary>Desembolsa los fondos del préstamo aprobado.</summary>
    public void Disburse()
    {
        if (Status != LoanStatus.Approved)
            throw new InvalidOperationException("Solo se pueden desembolsar préstamos aprobados.");

        Status = LoanStatus.Disbursed;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new LoanDisbursedDomainEvent(
            Id, ApprovedAmount!.Amount, UpdatedAt.Value));
    }

    /// <summary>Cancela el préstamo (compensación SAGA).</summary>
    public void Cancel(string reason)
    {
        if (Status is LoanStatus.Disbursed or LoanStatus.Cancelled)
            throw new InvalidOperationException($"No se puede cancelar un préstamo en estado {Status}.");

        RejectionReason = reason;
        Status = LoanStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new LoanCancelledDomainEvent(Id, reason, UpdatedAt.Value));
    }
}
