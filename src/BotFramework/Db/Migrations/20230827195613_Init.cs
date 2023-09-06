using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BotFramework.Db.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавить расширение для хранения словарей.
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS hstore;");
            
            migrationBuilder.EnsureSchema(
                name: "bot");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_username = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    telegram_firstname = table.Column<string>(type: "text", nullable: true),
                    telegram_lastname = table.Column<string>(type: "text", nullable: true),
                    __data = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: true),
                    telegram_username = table.Column<string>(type: "text", nullable: true),
                    bot_user_id = table.Column<long>(type: "bigint", nullable: false),
                    __data = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    __states = table.Column<List<string>>(type: "text[]", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_chats_users_bot_user_id",
                        column: x => x.bot_user_id,
                        principalSchema: "bot",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "updates",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bot_chat_id = table.Column<long>(type: "bigint", nullable: false),
                    telegram_message_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_updates", x => x.id);
                    table.ForeignKey(
                        name: "fk_updates_chats_bot_chat_id",
                        column: x => x.bot_chat_id,
                        principalSchema: "bot",
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exceptions",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    chat_id = table.Column<long>(type: "bigint", nullable: true),
                    update_id = table.Column<Guid>(type: "uuid", nullable: true),
                    exception_message = table.Column<string>(type: "text", nullable: true),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    report_description = table.Column<string>(type: "text", nullable: true),
                    report_file_name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_exceptions_chats_chat_id",
                        column: x => x.chat_id,
                        principalSchema: "bot",
                        principalTable: "chats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_exceptions_updates_update_id",
                        column: x => x.update_id,
                        principalSchema: "bot",
                        principalTable: "updates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_exceptions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "bot",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_chats_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_chat_id",
                schema: "bot",
                table: "exceptions",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_update_id",
                schema: "bot",
                table: "exceptions",
                column: "update_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_user_id",
                schema: "bot",
                table: "exceptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_updates_bot_chat_id",
                schema: "bot",
                table: "updates",
                column: "bot_chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_telegram_id",
                schema: "bot",
                table: "users",
                column: "telegram_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exceptions",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "updates",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "chats",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "users",
                schema: "bot");
        }
    }
}
