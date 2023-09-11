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
                name: "claims",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Имя разрешения."),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Описание разрешения."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_claims", x => x.id);
                },
                comment: "Таблица разрешений.");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор пользователя в Telegram."),
                    telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Ник пользователя в Telegram."),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "Номер телефона пользователя."),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Роль пользователя в боте. Например [user, moderator, admin]."),
                    language_code = table.Column<string>(type: "text", nullable: true, comment: "Код языка пользователя. Ссылка на коды [https://en.wikipedia.org/wiki/IETF_language_tag]"),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false, comment: "Флаг заблокированного пользователя."),
                    telegram_firstname = table.Column<string>(type: "text", nullable: true, comment: "Имя пользователя в Telegram."),
                    telegram_lastname = table.Column<string>(type: "text", nullable: true, comment: "Фамилия пользователя в Telegram."),
                    __properties_database_dictionary = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false, comment: "Словарь для хранения свойств пользователя (динамически)."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                },
                comment: "Таблица пользователей бота.");

            migrationBuilder.CreateTable(
                name: "chats",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: true, comment: "Идентификатор чата в Telegram."),
                    telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Строковый идентификатор чата в телеграм. Некоторые чаты вместо long идентификатора имеют username идентификатор."),
                    bot_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на пользователя."),
                    __data_database_dictionary = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    __states = table.Column<List<string>>(type: "text[]", nullable: false, comment: "Состояние чата."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
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
                },
                comment: "Сущность чата.");

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на пользователя."),
                    claim_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ разрешения."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_claims_claim_id",
                        column: x => x.claim_id,
                        principalSchema: "bot",
                        principalTable: "claims",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "bot",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Таблица сопоставлений пользователей и разрешений.");

            migrationBuilder.CreateTable(
                name: "updates",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Идентификатор сущности."),
                    bot_chat_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на таблицу чатов."),
                    telegram_message_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сообщения в Telegram чате."),
                    type = table.Column<string>(type: "text", nullable: false, comment: "Тип сообщения (текст, картинка, аудио, документ и т.д.)."),
                    content = table.Column<string>(type: "text", nullable: false, comment: "Содержимое сообщения."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
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
                },
                comment: "Таблица сообщений (запросов) бота.");

            migrationBuilder.CreateTable(
                name: "exceptions",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true, comment: "Кто отправил запрос боту, когда произошла ошибка."),
                    chat_id = table.Column<long>(type: "bigint", nullable: true, comment: "ИД чата, откуда пришел запрос, когда произошла ошибка."),
                    update_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "ИД запроса, в момент обработки которого произошла ошибка."),
                    exception_message = table.Column<string>(type: "text", nullable: true, comment: "Сообщение об ошибке."),
                    stack_trace = table.Column<string>(type: "text", nullable: true, comment: "Стек вызовов в приложении, перед ошибкой."),
                    report_description = table.Column<string>(type: "text", nullable: true, comment: "Отчет об ошибке."),
                    report_file_name = table.Column<string>(type: "text", nullable: true, comment: "Имя файла, в котором записан отчет об ошибке."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Дата и время создания сущности в БД."),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время последнего обновления сущности в БД."),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Дата и время удаления сущности в БД.")
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
                },
                comment: "Сообщения об ошибке в боте.");

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
                name: "ix_user_claims_claim_id",
                schema: "bot",
                table: "user_claims",
                column: "claim_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id_claim_id",
                schema: "bot",
                table: "user_claims",
                columns: new[] { "user_id", "claim_id" },
                unique: true);

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
                name: "user_claims",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "updates",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "claims",
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
