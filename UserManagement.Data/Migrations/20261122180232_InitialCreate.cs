using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ObjectType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectBeforeJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjectAfterJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Forename = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Surname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "Forename", "IsActive", "IsDeleted", "Surname" },
                values: new object[,]
                {
                    { 1L, new DateTime(1962, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ploew@example.com", "Peter", true, false, "Loew" },
                    { 2L, new DateTime(1970, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "bfgates@example.com", "Benjamin Franklin", true, false, "Gates" },
                    { 3L, new DateTime(1965, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "ctroy@example.com", "Castor", false, false, "Troy" },
                    { 4L, new DateTime(1974, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "mraines@example.com", "Memphis", true, false, "Raines" },
                    { 5L, new DateTime(1968, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "sgodspeed@example.com", "Stanley", true, false, "Goodspeed" },
                    { 6L, new DateTime(1972, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "himcdunnough@example.com", "H.I.", true, false, "McDunnough" },
                    { 7L, new DateTime(1976, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "cpoe@example.com", "Cameron", false, false, "Poe" },
                    { 8L, new DateTime(1981, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "emalus@example.com", "Edward", false, false, "Malus" },
                    { 9L, new DateTime(1970, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "dmacready@example.com", "Damon", false, false, "Macready" },
                    { 10L, new DateTime(1985, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "jblaze@example.com", "Johnny", true, false, "Blaze" },
                    { 11L, new DateTime(1990, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "rfeld@example.com", "Robin", true, false, "Feld" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
