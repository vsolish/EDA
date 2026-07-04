using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Commands.CreatePayment;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.Request.ReservationId)
            .NotEmpty().WithMessage("ReservationId is required");

        RuleFor(x => x.Request.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required")
            .MaximumLength(128).WithMessage("CustomerId cannot exceed 128 characters");

        RuleFor(x => x.Request.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Request.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO 4217 code");
    }
}
