using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061201)]
    public class CreateFileshardCollectionTable : Migration
    {
        public override void Up()
        {
           Create.Table("collections")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("Title").AsString();
        }

        public override void Down()
        {
            Delete.Table("collections");
        }
    }
}
