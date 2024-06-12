using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061202)]
    public class CreateFileshardObjectTable : Migration
    {
        public override void Up()
        {
            Create.Table("objects")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString()
                .WithColumn("IsImport").AsBoolean()
                .WithColumn("Version").AsGuid().NotNullable()
                .WithColumn("CollectionId").AsGuid().ForeignKey("collections", "id");
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
