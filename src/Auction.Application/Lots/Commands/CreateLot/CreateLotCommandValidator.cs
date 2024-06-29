using FluentValidation;

namespace Auction.Application.Lots.Commands.CreateLot;

public class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
{
    public CreateLotCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.StartPrice)
            .GreaterThan(10)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty();

    }
}
