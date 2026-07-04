using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Application.Commands.CreateRoom;

public class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Request.HotelId)
            .NotEmpty().WithMessage("HotelId is required")
            .MaximumLength(128).WithMessage("HotelId cannot exceed 128 characters");

        RuleFor(x => x.Request.RoomType)
            .NotEmpty().WithMessage("RoomType is required")
            .MaximumLength(64).WithMessage("RoomType cannot exceed 64 characters");

        RuleFor(x => x.Request.RoomNumber)
            .NotEmpty().WithMessage("RoomNumber is required")
            .MaximumLength(32).WithMessage("RoomNumber cannot exceed 32 characters");

        RuleFor(x => x.Request.PricePerNight)
            .GreaterThan(0).WithMessage("PricePerNight must be greater than zero");
    }
}
