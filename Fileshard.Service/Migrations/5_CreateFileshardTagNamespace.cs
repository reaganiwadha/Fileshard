using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061205)]
    public class CreateFileshardFileTagNamespace : Migration
    {
        public override void Up()
        {
            Create.Table("tag_namespaces")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Name").AsString();
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
