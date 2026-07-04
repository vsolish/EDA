using Hotel.Payments.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Payments.Application.Commands.CreatePayment;

public record CreatePaymentCommand(CreatePaymentRequest Request) : IRequest<PaymentDto>;