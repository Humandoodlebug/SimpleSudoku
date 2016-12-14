using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SC.SimpleSudoku;

namespace SC.SimpleSudoku.Migrations
{
    [DbContext(typeof(SudokuDataContext))]
    [Migration("20161212105025_InitialCreate")]
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

                    b.Property<string>("PuzzleProblemData");

                    b.Property<string>("PuzzleSolutionData");

                    b.HasKey("ID");

                    b.ToTable("BasePuzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Old_Password", b =>
                {
                    b.Property<string>("Username");

                    b.Property<string>("OldPassword");

                    b.Property<string>("Username1");

                    b.HasKey("Username", "OldPassword");

                    b.HasIndex("Username1");

                    b.ToTable("OldPasswords");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle", b =>
                {
                    b.Property<int>("Seed");

                    b.Property<int>("BasePuzzleID");

                    b.Property<int>("Difficulty");

                    b.HasKey("Seed", "BasePuzzleID");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("PuzzleSeed");

                    b.Property<int>("AttemptNum");

                    b.Property<DateTime>("DateTimeAttempted");

                    b.Property<DateTime>("DateTimeCompleted");

                    b.Property<int>("MistakeCount");

                    b.Property<int?>("PuzzleBasePuzzleID");

                    b.Property<int?>("PuzzleSeed1");

                    b.Property<int>("Score");

                    b.Property<TimeSpan>("SolvingTime");

                    b.Property<string>("Username1");

                    b.HasKey("Username", "PuzzleSeed", "AttemptNum");

                    b.HasIndex("Username1");

                    b.HasIndex("PuzzleSeed1", "PuzzleBasePuzzleID");

                    b.ToTable("PuzzleAttempts");
                });

            modelBuilder.Entity("SC.SimpleSudoku.User", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("AveragePuzzleDifficulty");

                    b.Property<int>("AverageScore");

                    b.Property<TimeSpan>("AverageSolvingTime");

                    b.Property<string>("CurrentPuzzleData");

                    b.Property<int>("CurrentPuzzleSeed");

                    b.Property<bool>("IsLeaderboardVisible");

                    b.Property<bool>("IsMistakeHighlightingOn");

                    b.Property<bool>("IsPuzzleTimerVisible");

                    b.Property<int>("NumPuzzlesSolved");

                    b.Property<string>("Password");

                    b.Property<long>("TotalScore");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Old_Password", b =>
                {
                    b.HasOne("SC.SimpleSudoku.User")
                        .WithMany("OldPasswords")
                        .HasForeignKey("Username1");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.HasOne("SC.SimpleSudoku.User")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("Username1");

                    b.HasOne("SC.SimpleSudoku.Puzzle")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("PuzzleSeed1", "PuzzleBasePuzzleID");
                });
        }
    }
}
