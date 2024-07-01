using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Users.Commands.GiveModeratorPermissions;

[Authorize(Role.Admin)]
public record SetModeratorRole(Guid UserId, bool IsActive) : IRequest<Result>;

public class SetModeratorRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SetModeratorRoleCommandHandler> logger)
        : IRequestHandler<SetModeratorRole, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SetModeratorRoleCommandHandler> _logger = logger;

    public async Task<Result> Handle(SetModeratorRole command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);

        if (user is null)
        {
            _logger.LogWarning("{Message}", UserError.NotFound.Description);
            return Result.Failure(UserError.NotFound);
        }

        user.Role = command.IsActive ? Role.Moderator : Role.User;

        await connection.Repositories.UserRepository.Update(user);
        await connection.SaveChangesAsync();

        return Result.Success();
    }
}
