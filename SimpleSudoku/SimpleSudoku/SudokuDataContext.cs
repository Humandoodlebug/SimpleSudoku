using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming

namespace SC.SimpleSudoku
{
    /// <summary>
    /// Used to specify the difficulty of a puzzle.
    /// </summary>
    public enum PuzzleDifficulty
    {
        Easy,
        Medium,
        Hard,
        Insane
    }

    /// <summary>
    /// This is for interfacing with the database
    /// </summary>
    public class SudokuDataContext : DbContext
    {
        //These 'DbSets' represent tables in the database. Inside the angle brackets (<>) are the names of some classes that hold properties pertaining to fields in the database. This way, data can be accessed through code and the database can be created 'code-first'.
        public DbSet<User> Users { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<BasePuzzle> BasePuzzles { get; set; }
        public DbSet<Mistake> Mistakes { get; set; }
        public DbSet<Puzzle_Attempt> PuzzleAttempts { get; set; }
        public DbSet<Old_Password> OldPasswords { get; set; }

        /// <summary>
        /// This method is called when the database is being configured. 
        /// It tells Entity Framework that we will be using a SQLite database with the file name "SudokuAppData.db".
        /// </summary>
        /// <param name="optionsBuilder">Used to specify the kind of database to be used</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SudokuAppData.db");
        }

        /// <summary>
        /// Called when the database model is being generated. Specifies relationships between tables and primary and foreign keys.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Old_Password>().HasKey(x => new {x.Username, x.OldPassword});
            modelBuilder.Entity<Puzzle_Attempt>().HasKey(x => new { x.Username, x.PuzzleSeed, x.BasePuzzleID, x.AttemptNum });
            modelBuilder.Entity<Mistake>().HasKey(x => new {x.Username, x.Row, x.Column});
            modelBuilder.Entity<Puzzle>().HasKey(x => new {x.PuzzleSeed, x.BasePuzzleId});
            modelBuilder.Entity<User>().HasKey(x => x.Username);
            modelBuilder.Entity<BasePuzzle>().HasKey(x => x.BasePuzzleID);
            modelBuilder.Entity<Puzzle>().HasMany(x => x.PuzzleAttempts).WithOne(x => x.Puzzle).HasForeignKey(x => new {x.PuzzleSeed, x.BasePuzzleID});
            modelBuilder.Entity<User>()
                .HasMany(x => x.CurrentPuzzleMistakes)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.Username);
            

        }
    }
    
    /// <summary>
    /// Represents records of the Users table in the database.
    /// </summary>
    public class User
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public int NumPuzzlesSolved { get; set; }
        public TimeSpan AverageSolvingTime { get; set; }
        public double AveragePuzzleDifficulty { get; set; }
        public double AverageScore { get; set; }
        public long TotalScore { get; set; }
        public int CurrentPuzzleSeed { get; set; }
        public string CurrentPuzzleData { get; set; }
        public bool IsMistakeHighlightingOn { get; set; } = true;
        public bool IsLeaderboardVisible { get; set; } = true;
        public bool IsPuzzleTimerVisible { get; set; } = true;
        public virtual ICollection<Puzzle_Attempt> PuzzleAttempts { get; set; }
        public virtual ICollection<Old_Password> OldPasswords { get; set; }
        public int CurrentBasePuzzleId { get; set; }
        public TimeSpan CurrentSolvingTime { get; set; }
        public virtual ICollection<Mistake> CurrentPuzzleMistakes { get; set; }
        public DateTime CurrentPuzzleStartTime { get; set; }
    }

    /// <summary>
    /// Represents records of the Old_Passwords table in the database.
    /// </summary>
    public class Old_Password
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public User User { get; set; }
    }

    /// <summary>
    /// Represents records of the Puzzles table in the database.
    /// </summary>
    public class Puzzle
    {
        public int PuzzleSeed { get; set; }
        public int BasePuzzleId { get; set; }
        public virtual ICollection<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }

    /// <summary>
    /// Represents records of the Puzzle_Attempts table in the database.
    /// </summary>
    public class Puzzle_Attempt
    {
        public string Username { get; set; }
        public int PuzzleSeed { get; set; }
        public int AttemptNum { get; set; }
        public DateTime DateTimeAttempted { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public TimeSpan SolvingTime { get; set; }
        public int MistakeCount { get; set; }
        public int Score { get; set; }
        public int BasePuzzleID { get; set; }

        
        public Puzzle Puzzle { get; set; }

        [ForeignKey(nameof(BasePuzzleID))]
        public BasePuzzle BasePuzzle { get; set; }
    }

    /// <summary>
    /// Represents records of the Base_Puzzles table in the database.
    /// </summary>
    public class BasePuzzle
    {
        [Key] public int BasePuzzleID { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }
        public string PuzzleProblemData { get; set; }
        public string PuzzleSolutionData { get; set; }

        public virtual ICollection<Puzzle_Attempt> PuzzleAttempts { get; set; }
    }

    /// <summary>
    /// Represents records if the Mistakes table in the database.
    /// </summary>
    public class Mistake
    {
        public string Username { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        [ForeignKey(nameof(Username))]
        public User User { get; set; }
    }
}