using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddInfoPropertiesToBotEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "bot",
                table: "bots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "short_description",
                schema: "bot",
                table: "bots",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                schema: "bot",
                table: "bots");

            migrationBuilder.DropColumn(
                name: "short_description",
                schema: "bot",
                table: "bots");
        }
    }
}
