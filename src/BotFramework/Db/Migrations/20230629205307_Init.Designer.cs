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
    [Migration("20230629205307_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BotFramework.Db.Entity.BotChat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BotUserId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_user_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<string>("TelegramUsername")
                        .HasColumnType("text")
                        .HasColumnName("telegram_username");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Dictionary<string, string>>("_data")
                        .IsRequired()
                        .HasColumnType("hstore")
                        .HasColumnName("__data");

                    b.Property<List<string>>("_states")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("__states");

                    b.HasKey("Id")
                        .HasName("pk_chats");

                    b.HasIndex("BotUserId")
                        .HasDatabaseName("ix_chats_bot_user_id");

                    b.ToTable("chats", "bot");
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUpdate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("BotChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("bot_chat_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<long>("TelegramMessageId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_message_id");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_updates");

                    b.HasIndex("BotChatId")
                        .HasDatabaseName("ix_updates_bot_chat_id");

                    b.ToTable("updates", "bot");
                });

            modelBuilder.Entity("BotFramework.Db.Entity.BotUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("role");

                    b.Property<string>("Status")
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("TelegramFirstname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_firstname");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<string>("TelegramLastname")
                        .HasColumnType("text")
                        .HasColumnName("telegram_lastname");

                    b.Property<string>("TelegramUsername")
                        .HasColumnType("text")
                        .HasColumnName("telegram_username");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Dictionary<string, string>>("_data")
                        .IsRequired()
                        .HasColumnType("hstore")
                        .HasColumnName("__data");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("TelegramId")
                        .HasDatabaseName("ix_users_telegram_id");

                    b.ToTable("users", "bot");
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
#pragma warning restore 612, 618
        }
    }
}
