using Hotel.Reservations.Application.DTOs;
using MediatR;

namespace Hotel.Reservations.Application.Commands.RejectReservation;

public record RejectReservationCommand(Guid Id, string Reason) : IRequest<ReservationDto>;
