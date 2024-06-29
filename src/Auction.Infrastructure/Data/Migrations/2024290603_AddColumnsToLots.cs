using FluentMigrator;

namespace Auction.Infrastructure.Data.Migrations;

[Migration(2024290603)]
public class AddColumnsToLots : Migration
{
    public override void Up()
    {
        Alter.Table("lots")
            .AddColumn("publisher_id").AsGuid().Nullable().ForeignKey("users", "id").OnDelete(System.Data.Rule.Cascade)
            .AddColumn("owner_id").AsGuid().Nullable().ForeignKey("users", "id").OnDelete(System.Data.Rule.Cascade)
            .AddColumn("archived").AsBoolean().WithDefaultValue(false).NotNullable();
    }

    public override void Down() {
        Delete.Column("publisher_id").FromTable("lots");
        Delete.Column("owner_id").FromTable("lots");
        Delete.Column("archived").FromTable("lots");
    }
}
