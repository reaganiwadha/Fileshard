using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061203)]
    public class CreateFileshardFileTable : Migration
    {
        public override void Up()
        {
           Create.Table("files")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ObjectId").AsGuid().ForeignKey("objects", "id")
                .WithColumn("InternalPath").AsString()
                .WithColumn("Version").AsGuid().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
