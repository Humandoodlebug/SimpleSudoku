using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming

namespace SimpleSudoku
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SudokuAppData.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Old_Password>().HasKey(x => new {x.Username, x.OldPassword});
            modelBuilder.Entity<Puzzle>().HasKey(x => x.Seed);
            modelBuilder.Entity<Puzzle_Attempt>().HasKey(x => new {x.Username, x.Seed});
            modelBuilder.Entity<User>().HasKey(x => x.Username);
        }
    }

    [Table(nameof(User))]
    public class User
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public DateTime AverageSolvingTime { get; set; }
        public int AveragePuzzleDifficulty { get; set; }
        public int AverageScore { get; set; }
        public int CurrentPuzzleSeed { get; set; }
        public string CurrentPuzzleData { get; set; }
        public List<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }

    [Table(nameof(Old_Password))]
    public class Old_Password
    {
        [Key, ForeignKey(nameof(User))] public string Username { get; set; }
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
        [Key, ForeignKey(nameof(User))] public string Username { get; set; }
        [Key, ForeignKey(nameof(Puzzle))] public int Seed { get; set; }
        [Key] public int AttemptNum { get; set; }
        public DateTime DateTimeAttempted { get; set; }
        public TimeSpan SolvingTime { get; set; }
        public int MistakeCount { get; set; }
        public int Score { get; set; }
    }
}