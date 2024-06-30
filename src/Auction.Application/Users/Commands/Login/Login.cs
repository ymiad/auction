using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Users.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<string>>;

public class LoginCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {

        using var connection = _unitOfWork.Create();

        var user = (await connection.Repositories.UserRepository.GetBy(nameof(User.Username), command.Username)).SingleOrDefault();

        if (user is null)
        {
            return Result<string>.Failure(AuthError.InvalidCredentials);
        }

        var isPasswordCorrect = PasswordHasher.IsPasswordCorrect(command.Password, user.Password, user.PasswordSalt);

        if (!isPasswordCorrect)
        {
            return Result<string>.Failure(AuthError.InvalidCredentials);
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.Id, user.Role);

        return Result<string>.Success(token);
    }
}
