using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024270601)]
public class AddLotsTable : Migration
{
    public override void Up()
    {
        Create.Table("lots")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString()
            .WithColumn("description").AsString()
            .WithColumn("start_price").AsDecimal()
            .WithColumn("trading_start_date").AsDateTime()
            .WithColumn("trading_end_date").AsDateTime();
    }

    public override void Down() {
        Delete.Table("lots");
    }
}
