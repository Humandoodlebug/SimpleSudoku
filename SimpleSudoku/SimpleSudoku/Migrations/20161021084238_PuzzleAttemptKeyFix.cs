using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleSudoku.Migrations
{
    public partial class PuzzleAttemptKeyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PuzzleAttempts",
                table: "PuzzleAttempts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PuzzleAttempts",
                table: "PuzzleAttempts",
                columns: new[] { "Username", "Seed" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PuzzleAttempts",
                table: "PuzzleAttempts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PuzzleAttempts",
                table: "PuzzleAttempts",
                column: "Username");
        }
    }
}
