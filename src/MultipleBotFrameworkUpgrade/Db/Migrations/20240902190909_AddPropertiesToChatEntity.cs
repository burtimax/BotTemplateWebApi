using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFrameworkUpgrade.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesToChatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats");

            migrationBuilder.AlterColumn<long>(
                name: "telegram_user_id",
                schema: "bot",
                table: "saved_messages",
                type: "bigint",
                nullable: true,
                comment: "Telegram ID пользователя.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Внешний ключ на таблицу чатов.");

            migrationBuilder.AlterColumn<long>(
                name: "telegram_chat_id",
                schema: "bot",
                table: "saved_messages",
                type: "bigint",
                nullable: true,
                comment: "Telegram ID чата.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Внешний ключ на таблицу чатов.");

            migrationBuilder.AlterColumn<long>(
                name: "bot_user_id",
                schema: "bot",
                table: "chats",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на пользователя.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "Внешний ключ на пользователя.");

            migrationBuilder.AddColumn<string>(
                name: "type",
                schema: "bot",
                table: "chats",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_bot_owners_bot_id",
                schema: "bot",
                table: "bot_owners",
                column: "bot_id");

            migrationBuilder.AddForeignKey(
                name: "fk_bot_owners_bots_bot_id",
                schema: "bot",
                table: "bot_owners",
                column: "bot_id",
                principalSchema: "bot",
                principalTable: "bots",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id",
                principalSchema: "bot",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bot_owners_bots_bot_id",
                schema: "bot",
                table: "bot_owners");

            migrationBuilder.DropForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "ix_bot_owners_bot_id",
                schema: "bot",
                table: "bot_owners");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "bot",
                table: "chats");

            migrationBuilder.AlterColumn<long>(
                name: "telegram_user_id",
                schema: "bot",
                table: "saved_messages",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на таблицу чатов.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Telegram ID пользователя.");

            migrationBuilder.AlterColumn<long>(
                name: "telegram_chat_id",
                schema: "bot",
                table: "saved_messages",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на таблицу чатов.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Telegram ID чата.");

            migrationBuilder.AlterColumn<long>(
                name: "bot_user_id",
                schema: "bot",
                table: "chats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Внешний ключ на пользователя.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Внешний ключ на пользователя.");

            migrationBuilder.AddForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id",
                principalSchema: "bot",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
