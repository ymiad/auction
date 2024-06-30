using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.BanUser;

[Authorize(Role.Moderator | Role.Admin)]

public record BanUserCommand(Guid UserId) : IRequest<Result>;

public class BanUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<BanUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(BanUserCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var user = await connection.Repositories.UserRepository.GetById(command.UserId);
        if (user is null)
        {
            return Result.Failure(UserError.NotFound);
        }

        user.Banned = true;

        await connection.Repositories.UserRepository.Update(user);
        await connection.SaveChangesAsync();

        return Result.Success();
    }
}