using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.Register;

public class RegisterCommand : IRequest<Guid>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
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
        var user = new User
        {
            Username = command.Username,
            Password = command.Password,
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
