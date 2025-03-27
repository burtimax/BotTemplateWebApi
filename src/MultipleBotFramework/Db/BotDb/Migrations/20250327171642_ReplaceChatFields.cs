using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.BotDb.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceChatFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "ix_chats_bot_user_id",
                schema: "bot",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "is_blocked",
                schema: "bot",
                table: "users");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "bot",
                table: "users");

            migrationBuilder.DropColumn(
                name: "bot_user_id",
                schema: "bot",
                table: "chats");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                schema: "bot",
                table: "chats",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Флаг заблокированного пользователя.");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "bot",
                table: "chats",
                type: "text",
                nullable: true,
                comment: "Статус пользователя");

            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                schema: "bot",
                table: "chats",
                type: "text[]",
                nullable: true,
                comment: "Теги чата, роли чата. Системное поле.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_blocked",
                schema: "bot",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "bot",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "tags",
                schema: "bot",
                table: "chats");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                schema: "bot",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Флаг заблокированного пользователя.");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "bot",
                table: "users",
                type: "text",
                nullable: true,
                comment: "Статус пользователя");

            migrationBuilder.AddColumn<long>(
                name: "bot_user_id",
                schema: "bot",
                table: "chats",
                type: "bigint",
                nullable: true,
                comment: "Внешний ключ на пользователя.");

            migrationBuilder.CreateIndex(
                name: "ix_chats_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_chats_users_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id",
                principalSchema: "bot",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
