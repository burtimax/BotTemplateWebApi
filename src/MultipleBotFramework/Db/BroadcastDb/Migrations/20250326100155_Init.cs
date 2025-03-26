using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BroadcastModule.Db.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "broadcast");

            migrationBuilder.CreateTable(
                name: "broadcast_tasks",
                schema: "broadcast",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bot_token = table.Column<string>(type: "text", nullable: false),
                    from_chat_id = table.Column<long>(type: "bigint", nullable: false),
                    from_message_id = table.Column<int>(type: "integer", nullable: false),
                    media_file_id = table.Column<string>(type: "text", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    reply_markup_json = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_broadcast_tasks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "broadcast_messages",
                schema: "broadcast",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    broadcast_task_id = table.Column<long>(type: "bigint", nullable: false),
                    error_log = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_broadcast_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_broadcast_messages_broadcast_tasks_broadcast_task_id",
                        column: x => x.broadcast_task_id,
                        principalSchema: "broadcast",
                        principalTable: "broadcast_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_broadcast_messages_broadcast_task_id",
                schema: "broadcast",
                table: "broadcast_messages",
                column: "broadcast_task_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "broadcast_messages",
                schema: "broadcast");

            migrationBuilder.DropTable(
                name: "broadcast_tasks",
                schema: "broadcast");
        }
    }
}
