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
                    UserUsername = table.Column<string>(nullable: false),
                    OldPassword = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPasswords", x => new { x.UserUsername, x.OldPassword });
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

            migrationBuilder.CreateTable(
                name: "PuzzleAttempts",
                columns: table => new
                {
                    UserUsername = table.Column<string>(nullable: false),
                    PuzzleSeed = table.Column<int>(nullable: false),
                    AttemptNum = table.Column<int>(nullable: false),
                    DateTimeAttempted = table.Column<DateTime>(nullable: false),
                    MistakeCount = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SolvingTime = table.Column<TimeSpan>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleAttempts", x => new { x.UserUsername, x.PuzzleSeed });
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Puzzles_PuzzleSeed",
                        column: x => x.PuzzleSeed,
                        principalTable: "Puzzles",
                        principalColumn: "Seed",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_PuzzleSeed",
                table: "PuzzleAttempts",
                column: "PuzzleSeed");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_Username",
                table: "PuzzleAttempts",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OldPasswords");

            migrationBuilder.DropTable(
                name: "PuzzleAttempts");

            migrationBuilder.DropTable(
                name: "Puzzles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
