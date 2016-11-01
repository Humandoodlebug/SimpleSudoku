using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SC.SimpleSudoku;

namespace SC.SimpleSudoku.Migrations
{
    [DbContext(typeof(SudokuDataContext))]
    [Migration("20161101164558_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("SC.SimpleSudoku.Base_Puzzle", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Difficulty");

                    b.HasKey("ID");

                    b.ToTable("BasePuzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Old_Password", b =>
                {
                    b.Property<string>("UserUsername");

                    b.Property<string>("OldPassword");

                    b.HasKey("UserUsername", "OldPassword");

                    b.ToTable("OldPasswords");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle", b =>
                {
                    b.Property<int>("Seed")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Difficulty");

                    b.HasKey("Seed");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.Property<string>("UserUsername");

                    b.Property<int>("PuzzleSeed");

                    b.Property<int>("AttemptNum");

                    b.Property<DateTime>("DateTimeAttempted");

                    b.Property<DateTime>("DateTimeCompleted");

                    b.Property<int>("MistakeCount");

                    b.Property<int>("Score");

                    b.Property<TimeSpan>("SolvingTime");

                    b.Property<string>("Username");

                    b.HasKey("UserUsername", "PuzzleSeed", "AttemptNum");

                    b.HasIndex("PuzzleSeed");

                    b.HasIndex("Username");

                    b.ToTable("PuzzleAttempts");
                });

            modelBuilder.Entity("SC.SimpleSudoku.User", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("AveragePuzzleDifficulty");

                    b.Property<int>("AverageScore");

                    b.Property<DateTime>("AverageSolvingTime");

                    b.Property<string>("CurrentPuzzleData");

                    b.Property<int>("CurrentPuzzleSeed");

                    b.Property<int>("NumPuzzlesSolved");

                    b.Property<string>("Password");

                    b.Property<long>("TotalScore");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.HasOne("SC.SimpleSudoku.Puzzle")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("PuzzleSeed")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SC.SimpleSudoku.User")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("Username");
                });
        }
    }
}
