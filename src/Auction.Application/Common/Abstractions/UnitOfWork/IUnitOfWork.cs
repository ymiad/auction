namespace Auction.Application.Common.Abstractions.UnitOfWork;

public interface IUnitOfWork
{
    IUnitOfWorkAdapter Create();
}
