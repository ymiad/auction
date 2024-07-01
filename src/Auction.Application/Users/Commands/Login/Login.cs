using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Application.Common.Options;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Auction.Application.Users.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<string>>;

public class LoginCommandHandler(IUnitOfWork unitOfWork, IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {

        using var connection = _unitOfWork.Create();

        var user = (await connection.Repositories.UserRepository.GetBy(nameof(User.Username), command.Username)).SingleOrDefault();

        if (user is null)
        {
            return Result<string>.Failure(AuthError.InvalidCredentials);
        }

        if (user.Banned)
        {
            return Result<string>.Failure(UserError.Banned);
        }

        var isPasswordCorrect = PasswordHasher.IsPasswordCorrect(command.Password, user.Password, user.PasswordSalt);

        if (!isPasswordCorrect)
        {
            return Result<string>.Failure(AuthError.InvalidCredentials);
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.Id, user.Role, _jwtOptions.Secret);

        return Result<string>.Success(token);
    }
}
