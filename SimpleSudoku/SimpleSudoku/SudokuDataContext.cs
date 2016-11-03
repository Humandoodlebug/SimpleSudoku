using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DbSet<Base_Puzzle> BasePuzzles { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SudokuAppData.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Old_Password>().HasKey(x => new {x.UserUsername, x.OldPassword});
            modelBuilder.Entity<Puzzle>().HasKey(x => x.Seed);
            modelBuilder.Entity<Puzzle_Attempt>().HasKey(x => new {x.UserUsername,x.PuzzleSeed, x.AttemptNum});
            modelBuilder.Entity<User>().HasKey(x => x.Username);
            modelBuilder.Entity<Base_Puzzle>().HasKey(x => x.ID);
        }
    }

    [Table(nameof(User))]
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
        public bool IsMistakeHighlightingActive { get; set; }
        public bool IsLeaderboardVisible { get; set; }
        public bool IsPuzzleTimerVisible { get; set; }
        public List<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }

    [Table(nameof(Old_Password))]
    public class Old_Password
    {
        [Key, ForeignKey(nameof(User))] public string UserUsername { get; set; }
        [Key] public string OldPassword { get; set; }
    }

    [Table(nameof(Puzzle))]
    public class Puzzle
    {
        [Key] public int Seed { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }
        public List<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }

    [Table(nameof(Puzzle_Attempt))]
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
        public int[,] PuzzleData;
    }
}