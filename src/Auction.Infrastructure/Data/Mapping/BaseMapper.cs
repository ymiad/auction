using Auction.Domain.Common;

namespace Auction.Infrastructure.Data.Mapping;

public abstract class BaseMapper<T> : IMapper<T> where T : BaseEntity
{
    private string _tableName;
    private Dictionary<string, string> _mapping;

    public BaseMapper(string tableName, Dictionary<string, string> mapping)
    {
        _tableName = tableName;
        _mapping = mapping;
    }

    public string GetTableName()
    {
        return _tableName;
    }

    public Dictionary<string, string> GetMapping()
    {
        return _mapping;
    }

    public string GetFieldName(string entityPropName)
    {
        return _mapping[entityPropName];
    }
}
