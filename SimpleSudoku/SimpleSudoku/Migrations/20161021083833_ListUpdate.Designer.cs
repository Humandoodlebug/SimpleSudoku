using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SimpleSudoku;

namespace SimpleSudoku.Migrations
{
    [DbContext(typeof(SudokuDataContext))]
    [Migration("20161021083833_ListUpdate")]
    partial class ListUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("SimpleSudoku.Old_Password", b =>
                {
                    b.Property<string>("Username");

                    b.Property<string>("OldPassword");

                    b.HasKey("Username", "OldPassword");

                    b.ToTable("OldPasswords");
                });

            modelBuilder.Entity("SimpleSudoku.Puzzle", b =>
                {
                    b.Property<int>("Seed")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Difficulty");

                    b.HasKey("Seed");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("AttemptNum");

                    b.Property<DateTime>("DateTimeAttempted");

                    b.Property<int>("MistakeCount");

                    b.Property<int>("Score");

                    b.Property<int>("Seed");

                    b.Property<TimeSpan>("SolvingTime");

                    b.Property<string>("Username1");

                    b.HasKey("Username");

                    b.HasIndex("Seed");

                    b.HasIndex("Username1");

                    b.ToTable("PuzzleAttempts");
                });

            modelBuilder.Entity("SimpleSudoku.User", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("AveragePuzzleDifficulty");

                    b.Property<int>("AverageScore");

                    b.Property<DateTime>("AverageSolvingTime");

                    b.Property<string>("CurrentPuzzleData");

                    b.Property<int>("CurrentPuzzleSeed");

                    b.Property<string>("Password");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.HasOne("SimpleSudoku.Puzzle")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("Seed")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SimpleSudoku.User")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("Username1");
                });
        }
    }
}
