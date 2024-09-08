using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToBotEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "bot",
                table: "bots",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "webhook",
                schema: "bot",
                table: "bots",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                schema: "bot",
                table: "bots");

            migrationBuilder.DropColumn(
                name: "webhook",
                schema: "bot",
                table: "bots");
        }
    }
}
