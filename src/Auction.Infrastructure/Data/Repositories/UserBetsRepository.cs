using Auction.Application.Common.Abstractions.Repository;
using Auction.Domain.Entities;
using Auction.Infrastructure.Data.Utils;
using Npgsql;
using System.Data;

namespace Auction.Infrastructure.Data.Repositories;

public class UserBetsRepository : BaseRepository<UserBet>, IUserBetRepository
{
    public UserBetsRepository(NpgsqlConnection connection, NpgsqlTransaction transaction) : base(connection, transaction) { }

    protected override UserBet Read(NpgsqlDataReader reader)
    {
        var mapper = Mapper.GetMapper<UserBet>();

        var result = new UserBet
        {
            Id = reader.GetGuid(mapper.GetFieldName(nameof(UserBet.Id))),
            LotId = reader.GetGuid(mapper.GetFieldName(nameof(UserBet.LotId))),
            UserId = reader.GetGuid(mapper.GetFieldName(nameof(UserBet.UserId))),
            Ammount = reader.GetDecimal(mapper.GetFieldName(nameof(UserBet.Ammount))),
        };

        return result;
    }
}
