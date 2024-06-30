using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public record GetLotsQuery : IRequest<List<LotDto>>;

public class GetLotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetLotsQuery, List<LotDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<List<LotDto>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var lots = await connection.Repositories.LotRepository.GetAll();
        return _mapper.Map<List<LotDto>>(lots);
    }
}
