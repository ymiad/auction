using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024290601)]
public class AddAccountsTable : Migration
{
    public override void Up()
    {
        Create.Table("accounts")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("ammount").AsDecimal();

        Alter.Table("users")
            .AddColumn("account_id")
            .AsGuid();

        Create
            .ForeignKey("FK_users_accounts")
            .FromTable("users")
            .ForeignColumn("account_id")
            .ToTable("accounts")
            .PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down() {
        Delete
            .ForeignKey("FK_users_accounts");

        Delete.Column("account_id")
            .FromTable("users");

        Delete.Table("accounts");
    }
}
