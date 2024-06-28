using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024270601)]
public class AddLotTable : Migration
{
    public override void Up()
    {
        Create.Table("lots")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString()
            .WithColumn("description").AsString()
            .WithColumn("start_price").AsDecimal()
            .WithColumn("trading_start").AsDateTime()
            .WithColumn("trading_duration").AsInt32();
    }

    public override void Down() {
        Delete.Table("lots");
    }
}
