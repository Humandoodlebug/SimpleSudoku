using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SC.SimpleSudoku;

namespace SC.SimpleSudoku.Migrations
{
    [DbContext(typeof(SudokuDataContext))]
    partial class SudokuDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("SC.SimpleSudoku.BasePuzzle", b =>
                {
                    b.Property<int>("BasePuzzleID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Difficulty");

                    b.Property<string>("PuzzleProblemData");

                    b.Property<string>("PuzzleSolutionData");

                    b.HasKey("BasePuzzleID");

                    b.ToTable("BasePuzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Mistake", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("Row");

                    b.Property<int>("Column");

                    b.HasKey("Username", "Row", "Column");

                    b.HasIndex("Username");

                    b.ToTable("Mistakes");
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
                    b.Property<int>("PuzzleSeed");

                    b.Property<int>("BasePuzzleId");

                    b.HasKey("PuzzleSeed", "BasePuzzleId");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.Property<string>("Username");

                    b.Property<int>("PuzzleSeed");

                    b.Property<int>("BasePuzzleID");

                    b.Property<int>("AttemptNum");

                    b.Property<DateTime>("DateTimeAttempted");

                    b.Property<DateTime>("DateTimeCompleted");

                    b.Property<int>("MistakeCount");

                    b.Property<int>("Score");

                    b.Property<TimeSpan>("SolvingTime");

                    b.Property<string>("Username1");

                    b.HasKey("Username", "PuzzleSeed", "BasePuzzleID", "AttemptNum");

                    b.HasIndex("BasePuzzleID");

                    b.HasIndex("Username1");

                    b.HasIndex("PuzzleSeed", "BasePuzzleID");

                    b.ToTable("PuzzleAttempts");
                });

            modelBuilder.Entity("SC.SimpleSudoku.User", b =>
                {
                    b.Property<string>("Username");

                    b.Property<double>("AveragePuzzleDifficulty");

                    b.Property<double>("AverageScore");

                    b.Property<TimeSpan>("AverageSolvingTime");

                    b.Property<int>("CurrentBasePuzzleId");

                    b.Property<string>("CurrentPuzzleData");

                    b.Property<int>("CurrentPuzzleSeed");

                    b.Property<DateTime>("CurrentPuzzleStartTime");

                    b.Property<TimeSpan>("CurrentSolvingTime");

                    b.Property<bool>("IsLeaderboardVisible");

                    b.Property<bool>("IsMistakeHighlightingOn");

                    b.Property<bool>("IsPuzzleTimerVisible");

                    b.Property<int>("NumPuzzlesSolved");

                    b.Property<string>("Password");

                    b.Property<long>("TotalScore");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Mistake", b =>
                {
                    b.HasOne("SC.SimpleSudoku.User", "User")
                        .WithMany("CurrentPuzzleMistakes")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SC.SimpleSudoku.Old_Password", b =>
                {
                    b.HasOne("SC.SimpleSudoku.User", "User")
                        .WithMany("OldPasswords")
                        .HasForeignKey("Username1");
                });

            modelBuilder.Entity("SC.SimpleSudoku.Puzzle_Attempt", b =>
                {
                    b.HasOne("SC.SimpleSudoku.BasePuzzle", "BasePuzzle")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("BasePuzzleID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SC.SimpleSudoku.User")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("Username1");

                    b.HasOne("SC.SimpleSudoku.Puzzle", "Puzzle")
                        .WithMany("PuzzleAttempts")
                        .HasForeignKey("PuzzleSeed", "BasePuzzleID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
