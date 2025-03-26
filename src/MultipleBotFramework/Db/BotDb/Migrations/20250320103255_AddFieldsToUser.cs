using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_bot",
                schema: "bot",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Является ли Telegram пользователь ботом.");

            migrationBuilder.AddColumn<bool>(
                name: "is_premium",
                schema: "bot",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Является ли аккаунт премиумом.");

            migrationBuilder.AddColumn<long>(
                name: "request_count",
                schema: "bot",
                table: "chats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Кол-во запросов в бота");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_bot",
                schema: "bot",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_premium",
                schema: "bot",
                table: "users");

            migrationBuilder.DropColumn(
                name: "request_count",
                schema: "bot",
                table: "chats");
        }
    }
}
