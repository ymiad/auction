using Auction.Domain.Common;
using Auction.Infrastructure.Data.Mapping;
using System.Reflection;
using System.Runtime.InteropServices;

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
    }
}
