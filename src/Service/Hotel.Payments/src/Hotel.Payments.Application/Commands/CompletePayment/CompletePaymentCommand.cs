using Hotel.Payments.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Commands.CompletePayment;

public record CompletePaymentCommand(Guid Id) : IRequest<PaymentDto>;
