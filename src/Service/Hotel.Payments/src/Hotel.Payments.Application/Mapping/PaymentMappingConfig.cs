using Hotel.Payments.Application.DTOs;
using Hotel.Payments.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Mapping;

/// <summary>
/// Configura explícitamente el mapeo Payment -> PaymentDto, ya que Money
/// es un Value Object anidado (Amount, Currency) que Mapster no aplana
/// automáticamente a propiedades de primer nivel sin esta configuración.
/// </summary>
public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentDto>()
            .Map(dest => dest.Amount, src => src.Money.Amount)
            .Map(dest => dest.Currency, src => src.Money.Currency)
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}