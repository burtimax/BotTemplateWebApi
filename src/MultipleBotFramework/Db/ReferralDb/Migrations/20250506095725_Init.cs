using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MultipleBotFramework.Db.ReferralDb.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "referral");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "participants",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bot_id = table.Column<long>(type: "bigint", nullable: false),
                    user_telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    referrer_code = table.Column<string>(type: "text", nullable: true),
                    __data_dictionary = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_participants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "Идентификатор сущности.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bot_id = table.Column<long>(type: "bigint", nullable: false),
                    participant_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    referral_count = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_companies", x => x.id);
                    table.ForeignKey(
                        name: "fk_companies_participants_participant_id",
                        column: x => x.participant_id,
                        principalSchema: "referral",
                        principalTable: "participants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_companies_participant_id",
                schema: "referral",
                table: "companies",
                column: "participant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companies",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "participants",
                schema: "referral");
        }
    }
}
