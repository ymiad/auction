using FluentValidation;

namespace Auction.Application.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(5);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(5);
    }
}
