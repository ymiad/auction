using Auction.Domain.Entities;

namespace Auction.Application.Common.Abstractions.Repository;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByCredentials(string username, string password);

    Task<User> GetByUsername(string username);
}
