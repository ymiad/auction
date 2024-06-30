using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Models;
using AutoMapper;

namespace Auction.Application.Lots.Queries.GetLots;

public record GetLotsQuery : IRequest<Result<List<LotDto>>>;

public class GetLotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetLotsQuery, Result<List<LotDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<List<LotDto>>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _unitOfWork.Create();
        var lots = await connection.Repositories.LotRepository.GetAll();
        return Result<List<LotDto>>.Success(_mapper.Map<List<LotDto>>(lots));
    }
}
