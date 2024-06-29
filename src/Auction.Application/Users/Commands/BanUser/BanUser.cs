using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.BanUser;

[Authorize(Role.Moderator | Role.Admin)]

public record BanUserCommand(Guid UserId) : IRequest<bool>;

public class BanUserCommandHandler : IRequestHandler<BanUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public BanUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(BanUserCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var user = await connection.Repositories.UserRepository.GetById(command.UserId);
        user.Banned = true;
        await connection.Repositories.UserRepository.Update(user);
        connection.SaveChanges();

        return true;
    }
}