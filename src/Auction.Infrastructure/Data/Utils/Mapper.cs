using Auction.Domain.Common;
using Auction.Infrastructure.Data.Constants;
using Auction.Infrastructure.Data.Mapping;
using System.Reflection;

namespace Auction.Infrastructure.Data.Utils
{
    public static class Mapper
    {
        public static Dictionary<string, string> GetMap<TEntity>() where TEntity : BaseEntity
        {
            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            var interfaceType = typeof(IMapping<TEntity>);
            var mappingType = assemblyTypes.FirstOrDefault(t => t.IsAssignableTo(interfaceType));
            var mapper = Activator.CreateInstance(mappingType);

            var dict = mappingType.GetMethods().First().Invoke(mapper, null) as Dictionary<string, string>;

            return dict;
        }

        public static string GetTableName<TEntity>(this IMapping<TEntity> mapping) where TEntity : BaseEntity
        {
            return mapping.GetMapping()[EntityConstants.TableName];
        }

        public static Dictionary<string, string> GetFields<TEntity>(this IMapping<TEntity> mapping) where TEntity : BaseEntity
        {
            return mapping.GetMapping().Where(x => x.Key != EntityConstants.TableName).ToDictionary();
        }

        public static string GetTableFieldName<TEntity>(this IMapping<TEntity> mapping, string entityPropName) where TEntity : BaseEntity
        {
            return mapping.GetMapping()[entityPropName];
        }

        public static string GetTableFieldName(Dictionary<string, string> mapping, string entityFieldName)
        {
            return mapping[entityFieldName];
        }
    }
}
