using FluentValidation;
using Hotel.Inventory.Application.Commands.UpdateRoom;

namespace IHotel.Inventory.Application.Commands.UpdateRoom;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Room ID is required");

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
