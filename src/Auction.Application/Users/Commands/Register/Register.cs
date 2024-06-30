using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.Register;

public record RegisterCommand : IRequest<Guid>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var (hashedPass, salt) = PasswordHasher.HashPassword(command.Password);

        var user = new User
        {
            Username = command.Username,
            Password = hashedPass,
            PasswordSalt = salt,
            Role = Role.User,
        };

        var account = new Account
        {
            Ammount = 0
        };

        Guid resultUserId = Guid.Empty;

        using (var connection = _unitOfWork.Create())
        {
            var accountId = await connection.Repositories.AccountRepository.Create(account);

            user.AccountId = accountId;
            resultUserId = await connection.Repositories.UserRepository.Create(user);
            connection.SaveChanges();
        }

        return resultUserId;
    }
}
