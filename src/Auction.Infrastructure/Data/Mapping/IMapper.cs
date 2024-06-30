using Auction.Domain.Common;

namespace Auction.Infrastructure.Data.Mapping;

public interface IMapper<T> where T : BaseEntity
{
    string GetTableName();
    string GetFieldName(string entityPropName);
    Dictionary<string, string> GetMapping();
}
