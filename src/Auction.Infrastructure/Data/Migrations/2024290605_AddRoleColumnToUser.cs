using Auction.Domain.Entities;
using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024290605)]
public class AddRoleColumnToUser : Migration
{
    public override void Up()
    {
        Alter.Table("users")
            .AddColumn("role").AsInt32().WithDefaultValue((int)Roles.User);
    }

    public override void Down() {
        Delete
            .Column("role")
            .FromTable("users");
    }
}
