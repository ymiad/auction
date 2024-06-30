using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.Login;

public record LoginCommand : IRequest<string>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
    {

        using var connection = _unitOfWork.Create();

        var user = (await connection.Repositories.UserRepository.GetBy(nameof(User.Username), command.Username)).SingleOrDefault();

        if (user is null)
        {
            return string.Empty;
        }

        var isPasswordCorrect = PasswordHasher.IsPasswordCorrect(command.Password, user.Password, user.PasswordSalt);

        if (!isPasswordCorrect)
        {
            return string.Empty;
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.Id, user.Role);

        return token;
    }
}
