using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleSudoku.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OldPasswords",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    OldPassword = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPasswords", x => new { x.Username, x.OldPassword });
                });

            migrationBuilder.CreateTable(
                name: "Puzzles",
                columns: table => new
                {
                    Seed = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Difficulty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puzzles", x => x.Seed);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleAttempts",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    AttemptNum = table.Column<int>(nullable: false),
                    DateTimeAttempted = table.Column<DateTime>(nullable: false),
                    MistakeCount = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    Seed = table.Column<int>(nullable: false),
                    SolvingTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleAttempts", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    AveragePuzzleDifficulty = table.Column<int>(nullable: false),
                    AverageScore = table.Column<int>(nullable: false),
                    AverageSolvingTime = table.Column<DateTime>(nullable: false),
                    CurrentPuzzleData = table.Column<string>(nullable: true),
                    CurrentPuzzleSeed = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OldPasswords");

            migrationBuilder.DropTable(
                name: "Puzzles");

            migrationBuilder.DropTable(
                name: "PuzzleAttempts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
