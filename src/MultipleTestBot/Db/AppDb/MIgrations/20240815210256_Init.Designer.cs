﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultipleTestBot.Db.AppDb;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MultipleTestBot.Db.AppDb.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240815210256_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MultipleTestBot.Db.AppDb.Entities.PostEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasComment("ИД сущности.");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasComment("Когда сущность была создана.");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by")
                        .HasComment("Кто создал сущность.");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at")
                        .HasComment("Когда сущность была удалена.");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("text")
                        .HasColumnName("deleted_by")
                        .HasComment("Кто удалил сущность.");

                    b.Property<string>("ImageFilename")
                        .HasColumnType("text")
                        .HasColumnName("image_filename");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("boolean")
                        .HasColumnName("is_hidden");

                    b.Property<int?>("Score")
                        .HasColumnType("integer")
                        .HasColumnName("score");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at")
                        .HasComment("Когда сущность была в последний раз обновлена.");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by")
                        .HasComment("Кто обновил сущность.");

                    b.HasKey("Id")
                        .HasName("pk_posts");

                    b.ToTable("posts", "app");
                });
#pragma warning restore 612, 618
        }
    }
}
