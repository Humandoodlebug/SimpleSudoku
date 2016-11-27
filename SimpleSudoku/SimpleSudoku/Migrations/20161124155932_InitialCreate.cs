using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SC.SimpleSudoku.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BasePuzzles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Difficulty = table.Column<int>(nullable: false),
                    PuzzleProblemData = table.Column<string>(nullable: true),
                    PuzzleSolutionData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasePuzzles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Puzzles",
                columns: table => new
                {
                    Seed = table.Column<int>(nullable: false),
                    BasePuzzleID = table.Column<int>(nullable: false),
                    Difficulty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puzzles", x => new { x.Seed, x.BasePuzzleID });
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
                    IsLeaderboardVisible = table.Column<bool>(nullable: false),
                    IsMistakeHighlightingOn = table.Column<bool>(nullable: false),
                    IsPuzzleTimerVisible = table.Column<bool>(nullable: false),
                    NumPuzzlesSolved = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    TotalScore = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "OldPasswords",
                columns: table => new
                {
                    UserUsername = table.Column<string>(nullable: false),
                    OldPassword = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPasswords", x => new { x.UserUsername, x.OldPassword });
                    table.ForeignKey(
                        name: "FK_OldPasswords_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleAttempts",
                columns: table => new
                {
                    UserUsername = table.Column<string>(nullable: false),
                    PuzzleSeed = table.Column<int>(nullable: false),
                    AttemptNum = table.Column<int>(nullable: false),
                    DateTimeAttempted = table.Column<DateTime>(nullable: false),
                    DateTimeCompleted = table.Column<DateTime>(nullable: false),
                    MistakeCount = table.Column<int>(nullable: false),
                    PuzzleBasePuzzleID = table.Column<int>(nullable: true),
                    PuzzleSeed1 = table.Column<int>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    SolvingTime = table.Column<TimeSpan>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleAttempts", x => new { x.UserUsername, x.PuzzleSeed, x.AttemptNum });
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Puzzles_PuzzleSeed1_PuzzleBasePuzzleID",
                        columns: x => new { x.PuzzleSeed1, x.PuzzleBasePuzzleID },
                        principalTable: "Puzzles",
                        principalColumns: new[] { "Seed", "BasePuzzleID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OldPasswords_Username",
                table: "OldPasswords",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_Username",
                table: "PuzzleAttempts",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_PuzzleSeed1_PuzzleBasePuzzleID",
                table: "PuzzleAttempts",
                columns: new[] { "PuzzleSeed1", "PuzzleBasePuzzleID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasePuzzles");

            migrationBuilder.DropTable(
                name: "OldPasswords");

            migrationBuilder.DropTable(
                name: "PuzzleAttempts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Puzzles");
        }
    }
}
