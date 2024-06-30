using Auction.Application.Common;
using Auction.Application.Common.Abstractions.UnitOfWork;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public record GetLotsQuery : IRequest<List<LotDto>>;

public class GetLotsQueryHandler : IRequestHandler<GetLotsQuery, List<LotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<LotDto>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var lots = await connection.Repositories.LotRepository.GetAll();
        return _mapper.Map<List<LotDto>>(lots);
    }
}
