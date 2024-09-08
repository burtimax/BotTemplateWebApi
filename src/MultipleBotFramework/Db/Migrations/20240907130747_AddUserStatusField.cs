using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatusField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "bot",
                table: "users",
                type: "text",
                nullable: true,
                comment: "Статус пользователя");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                schema: "bot",
                table: "users");
        }
    }
}
