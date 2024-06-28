using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;

namespace Auction.Application.Lots.Commands.CreateLot;

public class CreateLotCommand : IRequest<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }
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
        };

        using var connection = _unitOfWork.Create();

        await connection.Repositories.LotRepository.Create(entity);

        connection.SaveChanges();

        return Guid.NewGuid();
    }
}
