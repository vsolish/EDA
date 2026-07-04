using Hotel.Payments.Application.DTOs;
using MediatR;


namespace Hotel.Payments.Application.Queries.GetPaymentByReservationId;

public record GetPaymentByReservationIdQuery(Guid ReservationId) : IRequest<PaymentDto?>;
