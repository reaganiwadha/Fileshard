using Fileshard.Service.Entities;
using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061207)]
    public class CreateFileshardObjectTag : Migration
    {
        public override void Up()
        {
            Create.Table("object_tags")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("ObjectId").AsGuid().ForeignKey("objects", "Id")
                .WithColumn("TagId").AsGuid().ForeignKey("tags", "Id")
                .WithColumn("Weight").AsFloat().Nullable();
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
