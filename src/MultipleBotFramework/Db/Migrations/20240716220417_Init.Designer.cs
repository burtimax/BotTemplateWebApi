﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultipleBotFramework.Db;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MultipleBotFramework.Db.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20240716220417_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "hstore");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotChatEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<long>("BotUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_user_id")
                        .HasComment("Внешний ключ на пользователя.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id")
                        .HasComment("Идентификатор чата в Telegram.");

                    b.Property<string>("TelegramUsername")
                        .HasColumnType("text")
                        .HasColumnName("telegram_username")
                        .HasComment("Строковый идентификатор чата в телеграм. Некоторые чаты вместо long идентификатора имеют username идентификатор.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Dictionary<string, string>>("_dataDatabaseDictionary")
                        .IsRequired()
                        .HasColumnType("hstore")
                        .HasColumnName("__data_database_dictionary");

                    b.Property<List<string>>("_states")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("__states")
                        .HasComment("Состояние чата.");

                    b.HasKey("Id")
                        .HasName("pk_chats");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_chats_bot_id");

                    b.HasIndex("BotUserId")
                        .HasDatabaseName("ix_chats_bot_user_id");

                    b.ToTable("chats", "bot", t =>
                        {
                            t.HasComment("Сущность чата.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotClaimEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description")
                        .HasComment("Описание разрешения.");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name")
                        .HasComment("Имя разрешения.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_claims");

                    b.ToTable("claims", "bot", t =>
                        {
                            t.HasComment("Таблица разрешений.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("Username")
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_bots");

                    b.ToTable("bots", "bot");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotExceptionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<long?>("ChatEntityId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_entity_id")
                        .HasComment("ИД чата, откуда пришел запрос, когда произошла ошибка.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("ExceptionMessage")
                        .HasColumnType("text")
                        .HasColumnName("exception_message")
                        .HasComment("Сообщение об ошибке.");

                    b.Property<string>("ReportDescription")
                        .HasColumnType("text")
                        .HasColumnName("report_description")
                        .HasComment("Отчет об ошибке.");

                    b.Property<string>("ReportFileName")
                        .HasColumnType("text")
                        .HasColumnName("report_file_name")
                        .HasComment("Имя файла, в котором записан отчет об ошибке.");

                    b.Property<string>("StackTrace")
                        .HasColumnType("text")
                        .HasColumnName("stack_trace")
                        .HasComment("Стек вызовов в приложении, перед ошибкой.");

                    b.Property<Guid?>("UpdateEntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("update_entity_id")
                        .HasComment("ИД запроса, в момент обработки которого произошла ошибка.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long?>("UserEntityId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_entity_id")
                        .HasComment("Кто отправил запрос боту, когда произошла ошибка.");

                    b.HasKey("Id")
                        .HasName("pk_exceptions");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_exceptions_bot_id");

                    b.HasIndex("ChatEntityId")
                        .HasDatabaseName("ix_exceptions_chat_entity_id");

                    b.HasIndex("UpdateEntityId")
                        .HasDatabaseName("ix_exceptions_update_entity_id");

                    b.HasIndex("UserEntityId")
                        .HasDatabaseName("ix_exceptions_user_entity_id");

                    b.ToTable("exceptions", "bot", t =>
                        {
                            t.HasComment("Сообщения об ошибке в боте.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotOwnerEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("TelegramFirstname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_firstname");

                    b.Property<string>("TelegramLastname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_lastname");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserTelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_telegram_id");

                    b.Property<string>("Username")
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_bot_owners");

                    b.ToTable("bot_owners", "bot");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotSavedMessageEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment")
                        .HasComment("Комментарий к сообщению.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("MediaGroupId")
                        .HasColumnType("text")
                        .HasColumnName("media_group_id")
                        .HasComment("Медиа группа сообщения. Приадлежит ли сообщение медиа группе?");

                    b.Property<long?>("TelegramChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_chat_id")
                        .HasComment("Внешний ключ на таблицу чатов.");

                    b.Property<int>("TelegramMessageId")
                        .HasColumnType("integer")
                        .HasColumnName("telegram_message_id")
                        .HasComment("Идентификатор сообщения в Telegram чате.");

                    b.Property<string>("TelegramMessageJson")
                        .HasColumnType("text")
                        .HasColumnName("telegram_message_json")
                        .HasComment("Сериализованное в json сообщение.");

                    b.Property<long?>("TelegramUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_user_id")
                        .HasComment("Внешний ключ на таблицу чатов.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_saved_messages");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_saved_messages_bot_id");

                    b.ToTable("saved_messages", "bot", t =>
                        {
                            t.HasComment("Таблица сохраненных сообщений бота. Для последующего использования");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUpdateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id")
                        .HasComment("Внешний ключ на таблицу чатов.");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content")
                        .HasComment("Содержимое сообщения.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<long>("TelegramMessageId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_message_id")
                        .HasComment("Идентификатор сообщения в Telegram чате.");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type")
                        .HasComment("Тип сообщения (текст, картинка, аудио, документ и т.д.).");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_updates");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_updates_bot_id");

                    b.HasIndex("ChatId")
                        .HasDatabaseName("ix_updates_chat_id");

                    b.ToTable("updates", "bot", t =>
                        {
                            t.HasComment("Таблица сообщений (запросов) бота.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUserClaimEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<long>("ClaimId")
                        .HasColumnType("bigint")
                        .HasColumnName("claim_id")
                        .HasComment("Внешний ключ разрешения.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id")
                        .HasComment("Внешний ключ на пользователя.");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_user_claims_bot_id");

                    b.HasIndex("ClaimId")
                        .HasDatabaseName("ix_user_claims_claim_id");

                    b.HasIndex("UserId", "ClaimId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_claims_user_id_claim_id");

                    b.ToTable("user_claims", "bot", t =>
                        {
                            t.HasComment("Таблица сопоставлений пользователей и разрешений.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_id")
                        .HasComment("Внешний ключ на бота.");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_blocked")
                        .HasComment("Флаг заблокированного пользователя.");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("text")
                        .HasColumnName("language_code")
                        .HasComment("Код языка пользователя. Ссылка на коды [https://en.wikipedia.org/wiki/IETF_language_tag]");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone")
                        .HasComment("Номер телефона пользователя.");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("role")
                        .HasComment("Роль пользователя в боте. Например [user, moderator, admin].");

                    b.Property<string>("TelegramFirstname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_firstname")
                        .HasComment("Имя пользователя в Telegram.");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id")
                        .HasComment("Идентификатор пользователя в Telegram.");

                    b.Property<string>("TelegramLastname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_lastname")
                        .HasComment("Фамилия пользователя в Telegram.");

                    b.Property<string>("TelegramUsername")
                        .HasColumnType("text")
                        .HasColumnName("telegram_username")
                        .HasComment("Ник пользователя в Telegram.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Dictionary<string, string>>("_propertiesDatabaseDictionary")
                        .IsRequired()
                        .HasColumnType("hstore")
                        .HasColumnName("__properties_database_dictionary")
                        .HasComment("Словарь для хранения свойств пользователя (динамически).");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("BotId")
                        .HasDatabaseName("ix_users_bot_id");

                    b.HasIndex("TelegramId")
                        .HasDatabaseName("ix_users_telegram_id");

                    b.ToTable("users", "bot", t =>
                        {
                            t.HasComment("Таблица пользователей бота.");
                        });
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotChatEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chats_bots_bot_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotUserEntity", "BotUser")
                        .WithMany()
                        .HasForeignKey("BotUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chats_users_bot_user_id");

                    b.Navigation("Bot");

                    b.Navigation("BotUser");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotExceptionEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .HasConstraintName("fk_exceptions_bots_bot_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotChatEntity", "ChatEntity")
                        .WithMany()
                        .HasForeignKey("ChatEntityId")
                        .HasConstraintName("fk_exceptions_chats_chat_entity_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotUpdateEntity", "UpdateEntity")
                        .WithMany()
                        .HasForeignKey("UpdateEntityId")
                        .HasConstraintName("fk_exceptions_updates_update_entity_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotUserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserEntityId")
                        .HasConstraintName("fk_exceptions_users_user_entity_id");

                    b.Navigation("Bot");

                    b.Navigation("ChatEntity");

                    b.Navigation("UpdateEntity");

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotSavedMessageEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_saved_messages_bots_bot_id");

                    b.Navigation("Bot");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUpdateEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_updates_bots_bot_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotChatEntity", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_updates_chats_chat_id");

                    b.Navigation("Bot");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUserClaimEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_bots_bot_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotClaimEntity", "Claim")
                        .WithMany()
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_claims_claim_id");

                    b.HasOne("MultipleBotFramework.Db.Entity.BotUserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_users_user_id");

                    b.Navigation("Bot");

                    b.Navigation("Claim");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MultipleBotFramework.Db.Entity.BotUserEntity", b =>
                {
                    b.HasOne("MultipleBotFramework.Db.Entity.BotEntity", "Bot")
                        .WithMany()
                        .HasForeignKey("BotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_bots_bot_id");

                    b.Navigation("Bot");
                });
#pragma warning restore 612, 618
        }
    }
}