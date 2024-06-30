using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.RoleUpdate;

public record UpdateRoleCommand(Guid UserId, Role Role) : IRequest<Result>;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);

        if (user is null)
        {
            return Result.Failure(UserError.NotFound);
        }

        user.Role = command.Role;

        await connection.Repositories.UserRepository.Update(user);

        await connection.SaveChangesAsync();

        return Result.Success();
    }
}
