using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.BroadcastDb.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBotIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bot_id",
                schema: "broadcast",
                table: "broadcast_messages");

            migrationBuilder.AddColumn<long>(
                name: "bot_id",
                schema: "broadcast",
                table: "broadcast_tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bot_id",
                schema: "broadcast",
                table: "broadcast_tasks");

            migrationBuilder.AddColumn<long>(
                name: "bot_id",
                schema: "broadcast",
                table: "broadcast_messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
