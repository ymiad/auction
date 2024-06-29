using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.Register;

public class RegisterCommand : IRequest<Guid>
{
    public string Username { get; set; }
    public string Password { get; set; }
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
            Password = command.Password
        };

        using var connection = _unitOfWork.Create();

        await connection.Repositories.UserRepository.Create(user);

        connection.SaveChanges();

        return Guid.NewGuid();
    }
}
