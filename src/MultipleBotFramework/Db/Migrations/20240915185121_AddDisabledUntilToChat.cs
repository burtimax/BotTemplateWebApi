using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddDisabledUntilToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "disabled_until",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Бот не отвечает/не реагирует чату до определенного времени.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "disabled_until",
                schema: "bot",
                table: "chats");
        }
    }
}
