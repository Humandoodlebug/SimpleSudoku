using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SC.SimpleSudoku.Migrations
{
    public partial class UserOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLeaderboardVisible",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMistakeHighlightingActive",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPuzzleTimerVisible",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLeaderboardVisible",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMistakeHighlightingActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPuzzleTimerVisible",
                table: "Users");
        }
    }
}
