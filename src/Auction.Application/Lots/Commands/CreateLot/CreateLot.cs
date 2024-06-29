using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Lots.Commands.CreateLot;

public record CreateLotCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal StartPrice { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateLotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        var entity = new Lot
        {
            Name = request.Name,
            Description = request.Description,
            StartPrice = request.StartPrice,
            TradingStartDate = request.StartDate,
            TradingEndDate = request.EndDate,
        };

        using var connection = _unitOfWork.Create();

        await connection.Repositories.LotRepository.Create(entity);

        connection.SaveChanges();

        return Guid.NewGuid();
    }
}
