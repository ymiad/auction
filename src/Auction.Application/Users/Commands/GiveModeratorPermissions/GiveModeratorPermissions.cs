using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.GiveModeratorPermissions;

public record GiveModeratorPermissionsCommand(Guid UserId) : IRequest<bool>;

public class GiveModeratorPermissionsCommandHandler : IRequestHandler<GiveModeratorPermissionsCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public GiveModeratorPermissionsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(GiveModeratorPermissionsCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);
        user.Role = Role.Moderator;
        await connection.Repositories.UserRepository.Update(user);

        connection.SaveChanges();

        return true;
    }
}
