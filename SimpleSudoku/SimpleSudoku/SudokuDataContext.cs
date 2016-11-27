using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Windows.Storage;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming

namespace SC.SimpleSudoku
{
    public enum PuzzleDifficulty
    {
        Easy,
        Medium,
        Hard,
        Insane
    }

    public class SudokuDataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Old_Password> OldPasswords { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<Puzzle_Attempt> PuzzleAttempts { get; set; }
        public DbSet<Base_Puzzle> BasePuzzles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SudokuAppData.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Old_Password>().HasKey(x => new {x.UserUsername, x.OldPassword});
            modelBuilder.Entity<Puzzle>().HasKey(x => new {x.Seed, x.BasePuzzleID});
            modelBuilder.Entity<Puzzle_Attempt>().HasKey(x => new {x.UserUsername, x.PuzzleSeed, x.AttemptNum});
            modelBuilder.Entity<User>().HasKey(x => x.Username);
            modelBuilder.Entity<Base_Puzzle>().HasKey(x => x.ID);
        }
    }

    public class User
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public int NumPuzzlesSolved { get; set; }
        public DateTime AverageSolvingTime { get; set; }
        public int AveragePuzzleDifficulty { get; set; }
        public int AverageScore { get; set; }
        public long TotalScore { get; set; }
        public int CurrentPuzzleSeed { get; set; }
        public string CurrentPuzzleData { get; set; }
        public bool IsMistakeHighlightingOn { get; set; } = true;
        public bool IsLeaderboardVisible { get; set; } = true;
        public bool IsPuzzleTimerVisible { get; set; } = true;
        public List<Puzzle_Attempt> PuzzleAttempts { get; set; }
        public List<Old_Password> OldPasswords { get; set; }
    }


    public class Old_Password
    {
        [Key, ForeignKey(nameof(User))] public string UserUsername { get; set; }
        [Key] public string OldPassword { get; set; }
    }


    public class Puzzle
    {
        [Key] public int Seed { get; set; }
        [Key] public int BasePuzzleID { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }
        public List<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }


    public class Puzzle_Attempt
    {
        [Key, ForeignKey(nameof(User))] public string UserUsername { get; set; }
        [Key, ForeignKey(nameof(Puzzle))] public int PuzzleSeed { get; set; }
        [Key] public int AttemptNum { get; set; }
        public DateTime DateTimeAttempted { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public TimeSpan SolvingTime { get; set; }
        public int MistakeCount { get; set; }
        public int Score { get; set; }
    }

    public class Base_Puzzle
    {
        [Key] public int ID { get; set; }
        public int Difficulty { get; set; }
        public string PuzzleProblemData { get; set; }
        public string PuzzleSolutionData { get; set; }
    }
}