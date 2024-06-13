using FluentMigrator;

namespace Fileshard.Service.Migrations
{
    [Migration(2024061204)]
    public class CreateFileshardFileMetaTable : Migration
    {
        public override void Up()
        {
            Create.Table("file_metas")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("FileId").AsGuid().ForeignKey("files", "Id")
                .WithColumn("TimeValue").AsDateTime().Nullable()
                .WithColumn("LongValue").AsInt32().Nullable()
                .WithColumn("Version").AsGuid().NotNullable()
                .WithColumn("Key").AsString()
                .WithColumn("Value").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("files");
        }
    }
}
