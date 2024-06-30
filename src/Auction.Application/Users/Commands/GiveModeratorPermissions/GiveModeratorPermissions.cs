using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.GiveModeratorPermissions;

public record GiveModeratorPermissionsCommand(Guid UserId) : IRequest<Result>;

public class GiveModeratorPermissionsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GiveModeratorPermissionsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GiveModeratorPermissionsCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);

        if (user is null)
        {
            return Result.Failure(UserError.NotFound);
        }

        user.Role = Role.Moderator;

        await connection.Repositories.UserRepository.Update(user);
        await connection.SaveChangesAsync();

        return Result.Success();
    }
}
