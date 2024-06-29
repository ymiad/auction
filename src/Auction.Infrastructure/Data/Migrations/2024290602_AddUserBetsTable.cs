using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024290602)]
public class AddUserBetsTable : Migration
{
    public override void Up()
    {
        Create.Table("user_bets")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable().ForeignKey("users", "id").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("lot_id").AsGuid().NotNullable().ForeignKey("lots", "id").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("ammount").AsDecimal().NotNullable();
    }

    public override void Down() {
        Delete.Table("user_bets");
    }
}
