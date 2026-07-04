using Hotel.Reservations.Application.DTOs;
using MediatR;

namespace Hotel.Reservations.Application.Commands.CancelReservation;

public record CancelReservationCommand(Guid Id, string Reason) : IRequest<ReservationDto>;
