using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024300602)]
public class AddPassSaltColumnToUser : Migration
{
    public override void Up()
    {
        Alter.Table("users")
            .AddColumn("password_salt").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete
            .Column("password_salt")
            .FromTable("users");
    }
}
