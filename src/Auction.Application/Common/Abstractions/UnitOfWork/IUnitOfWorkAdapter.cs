namespace Auction.Application.Common.Abstractions.UnitOfWork;

public interface IUnitOfWorkAdapter : IDisposable, IAsyncDisposable
{
    IUnitOfWorkRepository Repositories { get; }
    Task SaveChangesAsync();
}
