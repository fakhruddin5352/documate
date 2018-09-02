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
                    Hash = table.Column<string>(type: "character(64)", nullable: true),
                    Owner = table.Column<string>(type: "character(40)", nullable: true),
                    When = table.Column<DateTime>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Hash",
                table: "Documents",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Owner",
                table: "Documents",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_When",
                table: "Documents",
                column: "When");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
