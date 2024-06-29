using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Commands.RoleUpdate;

public record UpdateRoleCommand : IRequest<bool>
{
    public Guid UserId { get; init; }
    public Role Role { get; init; }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetById(command.UserId);

        if (user is null)
        {
            return false;
        }

        user.Role = command.Role;

        await connection.Repositories.UserRepository.Update(user);

        connection.SaveChanges();

        return true;
    }
}
