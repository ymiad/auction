using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.Register;

public record RegisterCommand(string Username, string Password) : IRequest<Result<Guid>>;

public class RegisterCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(RegisterCommand command, CancellationToken cancellationToken)
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

        using var connection = _unitOfWork.Create();

        var createdAccountId = await connection.Repositories.AccountRepository.Create(account);

        user.AccountId = createdAccountId;

        var createdUserId = await connection.Repositories.UserRepository.Create(user);
        await connection.SaveChangesAsync();

        return Result<Guid>.Success(createdUserId);
    }
}
