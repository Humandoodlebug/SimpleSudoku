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
                    BasePuzzleID = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Difficulty = table.Column<int>(nullable: false),
                    PuzzleProblemData = table.Column<string>(nullable: true),
                    PuzzleSolutionData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasePuzzles", x => x.BasePuzzleID);
                });

            migrationBuilder.CreateTable(
                name: "Puzzles",
                columns: table => new
                {
                    PuzzleSeed = table.Column<int>(nullable: false),
                    BasePuzzleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puzzles", x => new { x.PuzzleSeed, x.BasePuzzleId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    AveragePuzzleDifficulty = table.Column<double>(nullable: false),
                    AverageScore = table.Column<double>(nullable: false),
                    AverageSolvingTime = table.Column<TimeSpan>(nullable: false),
                    CurrentBasePuzzleId = table.Column<int>(nullable: false),
                    CurrentPuzzleData = table.Column<string>(nullable: true),
                    CurrentPuzzleSeed = table.Column<int>(nullable: false),
                    CurrentPuzzleStartTime = table.Column<DateTime>(nullable: false),
                    CurrentSolvingTime = table.Column<TimeSpan>(nullable: false),
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
                name: "Mistakes",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Row = table.Column<int>(nullable: false),
                    Column = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mistakes", x => new { x.Username, x.Row, x.Column });
                    table.ForeignKey(
                        name: "FK_Mistakes_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OldPasswords",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    OldPassword = table.Column<string>(nullable: false),
                    Username1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OldPasswords", x => new { x.Username, x.OldPassword });
                    table.ForeignKey(
                        name: "FK_OldPasswords_Users_Username1",
                        column: x => x.Username1,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleAttempts",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    PuzzleSeed = table.Column<int>(nullable: false),
                    BasePuzzleID = table.Column<int>(nullable: false),
                    AttemptNum = table.Column<int>(nullable: false),
                    DateTimeAttempted = table.Column<DateTime>(nullable: false),
                    DateTimeCompleted = table.Column<DateTime>(nullable: false),
                    MistakeCount = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    SolvingTime = table.Column<TimeSpan>(nullable: false),
                    Username1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleAttempts", x => new { x.Username, x.PuzzleSeed, x.BasePuzzleID, x.AttemptNum });
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_BasePuzzles_BasePuzzleID",
                        column: x => x.BasePuzzleID,
                        principalTable: "BasePuzzles",
                        principalColumn: "BasePuzzleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Users_Username1",
                        column: x => x.Username1,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PuzzleAttempts_Puzzles_PuzzleSeed_BasePuzzleID",
                        columns: x => new { x.PuzzleSeed, x.BasePuzzleID },
                        principalTable: "Puzzles",
                        principalColumns: new[] { "PuzzleSeed", "BasePuzzleId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mistakes_Username",
                table: "Mistakes",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_OldPasswords_Username1",
                table: "OldPasswords",
                column: "Username1");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_BasePuzzleID",
                table: "PuzzleAttempts",
                column: "BasePuzzleID");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_Username1",
                table: "PuzzleAttempts",
                column: "Username1");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleAttempts_PuzzleSeed_BasePuzzleID",
                table: "PuzzleAttempts",
                columns: new[] { "PuzzleSeed", "BasePuzzleID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mistakes");

            migrationBuilder.DropTable(
                name: "OldPasswords");

            migrationBuilder.DropTable(
                name: "PuzzleAttempts");

            migrationBuilder.DropTable(
                name: "BasePuzzles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Puzzles");
        }
    }
}
