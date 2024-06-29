using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024300601)]
public class AddBannedToUser : Migration
{
    public override void Up()
    {
        Alter.Table("users")
            .AddColumn("banned").AsBoolean().WithDefaultValue(false);
    }

    public override void Down() {
        Delete
            .Column("banned")
            .FromTable("users");
    }
}
