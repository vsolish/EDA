using FluentValidation;

namespace Hotel.Payments.Application.Commands.FailPayment;

public class FailPaymentValidator : AbstractValidator<FailPaymentCommand>
{
    public FailPaymentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Payment ID is required");

        RuleFor(x => x.Request.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(512).WithMessage("Reason cannot exceed 512 characters");
    }
}
