using Auction.Domain.Common;

namespace Auction.Infrastructure.Data.Mapping
{
    public interface IMapping<T> where T : BaseEntity
    {
        Dictionary<string, string> GetMapping();
    }
}
