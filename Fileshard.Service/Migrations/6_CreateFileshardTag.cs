using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061206)]
    public class CreateFileshardFileTag : Migration
    {
        public override void Up()
        {
            Create.Table("tags")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("NamespaceId").AsGuid().ForeignKey("tag_namespaces", "Id")
                .WithColumn("Name").AsString();
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
