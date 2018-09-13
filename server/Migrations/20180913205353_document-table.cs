using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Documate.Migrations
{
    public partial class documenttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Owner = table.Column<string>(type: "character(40)", nullable: false),
                    When = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Verified = table.Column<string>(type: "character(40)", nullable: false),
                    Hash = table.Column<string>(type: "character(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_When",
                table: "Documents",
                column: "When");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Hash",
                table: "Documents",
                column: "Hash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
