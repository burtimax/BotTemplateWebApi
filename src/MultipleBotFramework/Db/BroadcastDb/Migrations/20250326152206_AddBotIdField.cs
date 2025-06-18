using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.BroadcastDb.Migrations
{
    /// <inheritdoc />
    public partial class AddBotIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "bot_id",
                schema: "broadcast",
                table: "broadcast_messages",
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
                table: "broadcast_messages");
        }
    }
}
