using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bot");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "bots",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "claims",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Имя разрешения."),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Описание разрешения."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_claims", x => x.id);
                },
                comment: "Таблица разрешений.");

            migrationBuilder.CreateTable(
                name: "bot_owners",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bot_id = table.Column<long>(type: "bigint", nullable: false),
                    user_telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "text", nullable: true),
                    telegram_firstname = table.Column<string>(type: "text", nullable: true),
                    telegram_lastname = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bot_owners", x => x.id);
                    table.ForeignKey(
                        name: "fk_bot_owners_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_messages",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_chat_id = table.Column<long>(type: "bigint", nullable: true, comment: "Telegram ID чата."),
                    telegram_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "Telegram ID пользователя."),
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор сообщения в Telegram чате."),
                    media_group_id = table.Column<string>(type: "text", nullable: true, comment: "Медиа группа сообщения. Приадлежит ли сообщение медиа группе?"),
                    telegram_message_json = table.Column<string>(type: "text", nullable: true, comment: "Сериализованное в json сообщение."),
                    comment = table.Column<string>(type: "text", nullable: true, comment: "Комментарий к сообщению."),
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_saved_messages_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Таблица сохраненных сообщений бота. Для последующего использования");

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
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    __properties_database_dictionary = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false, comment: "Словарь для хранения свойств пользователя (динамически)."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Таблица пользователей бота.");

            migrationBuilder.CreateTable(
                name: "chats",
                schema: "bot",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор чата в Telegram."),
                    type = table.Column<string>(type: "text", nullable: true),
                    telegram_username = table.Column<string>(type: "text", nullable: true, comment: "Строковый идентификатор чата в телеграм. Некоторые чаты вместо long идентификатора имеют username идентификатор."),
                    title = table.Column<string>(type: "text", nullable: true, comment: "Заголовок чата"),
                    bot_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "Внешний ключ на пользователя."),
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    __data_database_dictionary = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    __states = table.Column<List<string>>(type: "text[]", nullable: false, comment: "Состояние чата."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_chats_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chats_users_bot_user_id",
                        column: x => x.bot_user_id,
                        principalSchema: "bot",
                        principalTable: "users",
                        principalColumn: "id");
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
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    chat_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на таблицу чатов."),
                    telegram_message_id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сообщения в Telegram чате."),
                    type = table.Column<string>(type: "text", nullable: false, comment: "Тип сообщения (текст, картинка, аудио, документ и т.д.)."),
                    content = table.Column<string>(type: "text", nullable: false, comment: "Содержимое сообщения."),
                    bot_id = table.Column<long>(type: "bigint", nullable: false, comment: "Внешний ключ на бота."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_updates", x => x.id);
                    table.ForeignKey(
                        name: "fk_updates_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_updates_chats_chat_id",
                        column: x => x.chat_id,
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
                    user_entity_id = table.Column<long>(type: "bigint", nullable: true, comment: "Кто отправил запрос боту, когда произошла ошибка."),
                    chat_entity_id = table.Column<long>(type: "bigint", nullable: true, comment: "ИД чата, откуда пришел запрос, когда произошла ошибка."),
                    update_entity_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "ИД запроса, в момент обработки которого произошла ошибка."),
                    exception_message = table.Column<string>(type: "text", nullable: true, comment: "Сообщение об ошибке."),
                    stack_trace = table.Column<string>(type: "text", nullable: true, comment: "Стек вызовов в приложении, перед ошибкой."),
                    report_description = table.Column<string>(type: "text", nullable: true, comment: "Отчет об ошибке."),
                    report_file_name = table.Column<string>(type: "text", nullable: true, comment: "Имя файла, в котором записан отчет об ошибке."),
                    bot_id = table.Column<long>(type: "bigint", nullable: true, comment: "Внешний ключ на бота."),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_exceptions_bots_bot_id",
                        column: x => x.bot_id,
                        principalSchema: "bot",
                        principalTable: "bots",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_exceptions_chats_chat_entity_id",
                        column: x => x.chat_entity_id,
                        principalSchema: "bot",
                        principalTable: "chats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_exceptions_updates_update_entity_id",
                        column: x => x.update_entity_id,
                        principalSchema: "bot",
                        principalTable: "updates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_exceptions_users_user_entity_id",
                        column: x => x.user_entity_id,
                        principalSchema: "bot",
                        principalTable: "users",
                        principalColumn: "id");
                },
                comment: "Сообщения об ошибке в боте.");

            migrationBuilder.CreateIndex(
                name: "ix_bot_owners_bot_id",
                schema: "bot",
                table: "bot_owners",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_chats_bot_id",
                schema: "bot",
                table: "chats",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_chats_bot_user_id",
                schema: "bot",
                table: "chats",
                column: "bot_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_bot_id",
                schema: "bot",
                table: "exceptions",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_chat_entity_id",
                schema: "bot",
                table: "exceptions",
                column: "chat_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_update_entity_id",
                schema: "bot",
                table: "exceptions",
                column: "update_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_exceptions_user_entity_id",
                schema: "bot",
                table: "exceptions",
                column: "user_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_messages_bot_id",
                schema: "bot",
                table: "saved_messages",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_updates_bot_id",
                schema: "bot",
                table: "updates",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_updates_chat_id",
                schema: "bot",
                table: "updates",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_bot_id",
                schema: "bot",
                table: "user_claims",
                column: "bot_id");

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
                name: "ix_users_bot_id",
                schema: "bot",
                table: "users",
                column: "bot_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_telegram_id",
                schema: "bot",
                table: "users",
                column: "telegram_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bot_owners",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "exceptions",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "saved_messages",
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

            migrationBuilder.DropTable(
                name: "bots",
                schema: "bot");
        }
    }
}
