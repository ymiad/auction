namespace Auction.Application.Common.Abstractions.UnitOfWork;

public interface IUnitOfWorkAdapter : IDisposable
{
    IUnitOfWorkRepository Repositories { get; }

    void Dispose();
    void SaveChanges();
}
