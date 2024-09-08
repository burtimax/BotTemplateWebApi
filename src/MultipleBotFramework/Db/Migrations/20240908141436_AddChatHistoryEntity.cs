using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddChatHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_history",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности."),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: false, comment: "ИД чата в телеграме."),
                    media_group_id = table.Column<string>(type: "text", nullable: true, comment: "Принадлежность сообщения определенной медиагруппе."),
                    message_id = table.Column<long>(type: "bigint", nullable: true, comment: "Идентификатор сообщения в Telegram чате."),
                    message_type = table.Column<string>(type: "text", nullable: true, comment: "Идентификатор сообщения в Telegram чате."),
                    type = table.Column<int>(type: "integer", nullable: false, comment: "Тип элемента истории: message, callback, membertype and ect..."),
                    content = table.Column<string>(type: "text", nullable: true, comment: "Строковое представление содержимого сообщения."),
                    json_object = table.Column<string>(type: "text", nullable: true, comment: "JSON представление содержимого сообщения."),
                    file_id = table.Column<string>(type: "text", nullable: true),
                    is_bot = table.Column<bool>(type: "boolean", nullable: false),
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_history_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "История чата.");

            migrationBuilder.CreateIndex(
                name: "ix_chat_history_bot_id",
                schema: "bot",
                table: "chat_history",
                column: "bot_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_history",
                schema: "bot");
        }
    }
}
