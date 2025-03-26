using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesToChatHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "json_object",
                schema: "bot",
                table: "chat_history",
                newName: "json_data");

            migrationBuilder.RenameColumn(
                name: "content",
                schema: "bot",
                table: "chat_history",
                newName: "text");

            migrationBuilder.AddColumn<string>(
                name: "inline_keyboard",
                schema: "bot",
                table: "chat_history",
                type: "text",
                nullable: true,
                comment: "JSON представление inline клавиатуры сообщения.");

            migrationBuilder.AddColumn<string>(
                name: "reply_keyboard",
                schema: "bot",
                table: "chat_history",
                type: "text",
                nullable: true,
                comment: "JSON представление клавиатуры пользователя.");

            migrationBuilder.AddColumn<long>(
                name: "reply_to_message_id",
                schema: "bot",
                table: "chat_history",
                type: "bigint",
                nullable: true,
                comment: "Ответ на сообщение. ИД сообщения, на которое отвечаем. Telegram идентификатор.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inline_keyboard",
                schema: "bot",
                table: "chat_history");

            migrationBuilder.DropColumn(
                name: "reply_keyboard",
                schema: "bot",
                table: "chat_history");

            migrationBuilder.DropColumn(
                name: "reply_to_message_id",
                schema: "bot",
                table: "chat_history");

            migrationBuilder.RenameColumn(
                name: "text",
                schema: "bot",
                table: "chat_history",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "json_data",
                schema: "bot",
                table: "chat_history",
                newName: "json_object");
        }
    }
}
