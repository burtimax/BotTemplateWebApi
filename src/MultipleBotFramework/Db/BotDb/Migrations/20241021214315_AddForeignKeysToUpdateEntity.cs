using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysToUpdateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_updates_chats_chat_id",
                schema: "bot",
                table: "updates");

            migrationBuilder.AlterColumn<long>(
                name: "chat_id",
                schema: "bot",
                table: "updates",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на таблицу чатов.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "Внешний ключ на таблицу чатов.");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                schema: "bot",
                table: "updates",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на таблицу пользователей.");

            migrationBuilder.CreateIndex(
                name: "ix_updates_user_id",
                schema: "bot",
                table: "updates",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_updates_chats_chat_id",
                schema: "bot",
                table: "updates",
                column: "chat_id",
                principalSchema: "bot",
                principalTable: "chats",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_updates_users_user_id",
                schema: "bot",
                table: "updates",
                column: "user_id",
                principalSchema: "bot",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_updates_chats_chat_id",
                schema: "bot",
                table: "updates");

            migrationBuilder.DropForeignKey(
                name: "fk_updates_users_user_id",
                schema: "bot",
                table: "updates");

            migrationBuilder.DropIndex(
                name: "ix_updates_user_id",
                schema: "bot",
                table: "updates");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "bot",
                table: "updates");

            migrationBuilder.AlterColumn<long>(
                name: "chat_id",
                schema: "bot",
                table: "updates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Внешний ключ на таблицу чатов.",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Внешний ключ на таблицу чатов.");

            migrationBuilder.AddForeignKey(
                name: "fk_updates_chats_chat_id",
                schema: "bot",
                table: "updates",
                column: "chat_id",
                principalSchema: "bot",
                principalTable: "chats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
