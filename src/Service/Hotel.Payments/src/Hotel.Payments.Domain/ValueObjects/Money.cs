using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un monto monetario. Es el equivalente,
/// en este dominio, a CreditLine (Customer), StayPeriod (Reservation) o
/// RoomAvailability (Room): encapsula su propia invariante para que un
/// Payment con monto inválido o sin moneda nunca pueda existir.
/// </summary>
public sealed class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "El monto debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("La moneda es obligatoria.", nameof(currency));

        if (currency.Length != 3)
        {
            throw new ArgumentException(
                "La moneda debe ser un código ISO 4217 de 3 letras (por ejemplo, PEN, USD).", nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public override bool Equals(object? obj) =>
        obj is Money other && Amount == other.Amount && Currency == other.Currency;

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
