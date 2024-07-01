using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Auction.Application.Users.Commands.RoleUpdate;

[Authorize(Role.Admin)]
public record UpdateRoleCommand(Guid UserId, Role Role) : IRequest<Result>;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRoleCommandHandler> logger)
    : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UpdateRoleCommandHandler> _logger = logger;

    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);

        if (user is null)
        {
            _logger.LogWarning("{Message}", UserError.NotFound.Description);
            return Result.Failure(UserError.NotFound);
        }

        user.Role = command.Role;

        await connection.Repositories.UserRepository.Update(user);

        await connection.SaveChangesAsync();

        return Result.Success();
    }
}
