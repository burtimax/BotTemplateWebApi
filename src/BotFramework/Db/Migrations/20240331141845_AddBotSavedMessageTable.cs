using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddBotSavedMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время последнего обновления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Дата и время удаления сущности в БД.");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldComment: "Дата и время создания сущности в БД.");

            migrationBuilder.CreateTable(
                name: "saved_messages",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: true, comment: "Внешний ключ на таблицу чатов."),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "Внешний ключ на таблицу чатов."),
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор сообщения в Telegram чате."),
                    media_group_id = table.Column<string>(type: "text", nullable: true, comment: "Медиа группа сообщения. Приадлежит ли сообщение медиа группе?"),
                    telegram_message_json = table.Column<string>(type: "text", nullable: true, comment: "Сериализованное в json сообщение."),
                    comment = table.Column<string>(type: "text", nullable: true, comment: "Комментарий к сообщению."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_messages", x => x.id);
                },
                comment: "Таблица сохраненных сообщений бота. Для последующего использования");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saved_messages",
                schema: "bot");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "user_claims",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "updates",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "exceptions",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "claims",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время последнего обновления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Дата и время удаления сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                schema: "bot",
                table: "chats",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Дата и время создания сущности в БД.",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }
    }
}
