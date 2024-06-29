using Auction.Domain.Common;

namespace Auction.Application.Common.Abstractions.Repository;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<IList<T>> GetAll();
    Task<Guid> Create(T entity);
    Task Update(T entity);
    Task<T> GetById(Guid id);
    Task<IList<T>> GetBy(string fieldName, object value);
    Task Delete(Guid id);
}
