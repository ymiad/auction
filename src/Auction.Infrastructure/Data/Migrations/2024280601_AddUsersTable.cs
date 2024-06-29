using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024280601)]
public class AddUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("username").AsString()
            .WithColumn("password").AsString();
    }

    public override void Down() {
        Delete.Table("users");
    }
}
