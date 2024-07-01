using Auction.Domain.Common;
using Auction.Infrastructure.Data.Mapping;
using System.Reflection;

namespace Auction.Infrastructure.Data.Utils;

public static class Mapper
{
    public static IMapper<TEntity> GetMapper<TEntity>() where TEntity : BaseEntity
    {
        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
        var interfaceType = typeof(IMapper<TEntity>);
        var mappingType = assemblyTypes.FirstOrDefault(t => t.IsAssignableTo(interfaceType));
        if (mappingType == null)
        {
            return null!;
        }

        var mapper = Activator.CreateInstance(mappingType);

        return (mapper as IMapper<TEntity>)!;
    }
}
