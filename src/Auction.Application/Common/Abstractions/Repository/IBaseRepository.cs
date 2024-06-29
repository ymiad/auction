using Auction.Domain.Common;

namespace Auction.Application.Common.Abstractions.Repository;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<IList<T>> GetAll();
    Task Create(T entity);
    Task<T> GetById(Guid id);
}
