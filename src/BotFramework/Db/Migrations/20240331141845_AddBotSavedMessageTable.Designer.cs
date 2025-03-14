﻿// <auto-generated />
using System;
using System.Collections.Generic;
using BotFramework.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BotFramework.Db.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20240331141845_AddBotSavedMessageTable")]
    partial class AddBotSavedMessageTable
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

            modelBuilder.Entity("BotFramework.Db.Entity.BotChat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

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

                    b.HasIndex("BotUserId")
                        .HasDatabaseName("ix_chats_bot_user_id");

                    b.ToTable("chats", "bot", t =>
                        {
                            t.HasComment("Сущность чата.");
                        });
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotClaim", b =>
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

            modelBuilder.Entity("BotFramework.Db.Entity.BotException", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id")
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

                    b.Property<Guid?>("UpdateId")
                        .HasColumnType("uuid")
                        .HasColumnName("update_id")
                        .HasComment("ИД запроса, в момент обработки которого произошла ошибка.");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id")
                        .HasComment("Кто отправил запрос боту, когда произошла ошибка.");

                    b.HasKey("Id")
                        .HasName("pk_exceptions");

                    b.HasIndex("ChatId")
                        .HasDatabaseName("ix_exceptions_chat_id");

                    b.HasIndex("UpdateId")
                        .HasDatabaseName("ix_exceptions_update_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_exceptions_user_id");

                    b.ToTable("exceptions", "bot", t =>
                        {
                            t.HasComment("Сообщения об ошибке в боте.");
                        });
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotSavedMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

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

                    b.ToTable("saved_messages", "bot", t =>
                        {
                            t.HasComment("Таблица сохраненных сообщений бота. Для последующего использования");
                        });
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUpdate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    b.Property<long>("BotChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_chat_id")
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

                    b.HasIndex("BotChatId")
                        .HasDatabaseName("ix_updates_bot_chat_id");

                    b.ToTable("updates", "bot", t =>
                        {
                            t.HasComment("Таблица сообщений (запросов) бота.");
                        });
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUser", b =>
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

                    b.HasIndex("TelegramId")
                        .HasDatabaseName("ix_users_telegram_id");

                    b.ToTable("users", "bot", t =>
                        {
                            t.HasComment("Таблица пользователей бота.");
                        });
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUserClaim", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("Идентификатор сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

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

            modelBuilder.Entity("BotFramework.Db.Entity.BotChat", b =>
                {
                    b.HasOne("BotFramework.Db.Entity.BotUser", "BotUser")
                        .WithMany()
                        .HasForeignKey("BotUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_chats_users_bot_user_id");

                    b.Navigation("BotUser");
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotException", b =>
                {
                    b.HasOne("BotFramework.Db.Entity.BotChat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .HasConstraintName("fk_exceptions_chats_chat_id");

                    b.HasOne("BotFramework.Db.Entity.BotUpdate", "Update")
                        .WithMany()
                        .HasForeignKey("UpdateId")
                        .HasConstraintName("fk_exceptions_updates_update_id");

                    b.HasOne("BotFramework.Db.Entity.BotUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_exceptions_users_user_id");

                    b.Navigation("Chat");

                    b.Navigation("Update");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUpdate", b =>
                {
                    b.HasOne("BotFramework.Db.Entity.BotChat", "Chat")
                        .WithMany()
                        .HasForeignKey("BotChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_updates_chats_bot_chat_id");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUserClaim", b =>
                {
                    b.HasOne("BotFramework.Db.Entity.BotClaim", "Claim")
                        .WithMany()
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_claims_claim_id");

                    b.HasOne("BotFramework.Db.Entity.BotUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_users_user_id");

                    b.Navigation("Claim");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
