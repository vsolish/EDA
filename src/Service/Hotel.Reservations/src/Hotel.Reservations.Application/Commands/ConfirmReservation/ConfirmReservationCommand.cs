using Hotel.Reservations.Application.DTOs;
using MediatR;

namespace Hotel.Reservations.Application.Commands.ConfirmReservation;

public record ConfirmReservationCommand(Guid Id) : IRequest<ReservationDto>;
