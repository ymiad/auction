using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using Auction.Domain.Entities;

namespace Auction.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<Result<List<User>>>;

public class GetUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersQuery, Result<List<User>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<User>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var users = await connection.Repositories.UserRepository.GetAll();
        return Result<List<User>>.Success([.. users]);
    }
}
