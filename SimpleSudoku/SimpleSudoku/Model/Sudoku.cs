using System;

namespace SC.SimpleSudoku.Model
{
    internal class Sudoku
    {
        /// <summary>
        /// Holds the base puzzle that the Sudoku puzzle is generated from.
        /// </summary>
        public BasePuzzle BasePuzzle { get; }

        /// <summary>
        /// Holds the problem data of the Sudoku puzzle.
        /// </summary>
        public byte[,] ProblemData { get; }

        /// <summary>
        /// Holds the Sudoku puzzle's solution.
        /// </summary>
        public byte[,] SolutionData { get; }

        /// <summary>
        /// Holds the Sudoku puzzle's seed.
        /// </summary>
        public int Seed { get; }

        /// <summary>
        /// The Sudoku constructor.
        /// </summary>
        /// <param name="basePuzzle">The base puzzle from which to generate the Sudoku puzzle.</param>
        /// <param name="seed">The seed to use in generating the Sudoku puzzle.</param>
        public Sudoku(BasePuzzle basePuzzle, int? seed = null)
        {
            BasePuzzle = basePuzzle;
            //If the seed is not specified, generate a new random seed.
            if (seed == null)
                seed = new Random().Next();
            Seed = seed.Value;
            //Generate Problem and Solution data for the puzzle.
            ProblemData = GeneratePuzzle(basePuzzle.PuzzleProblemData, Seed);
            SolutionData = GeneratePuzzle(basePuzzle.PuzzleSolutionData, Seed);
        }

        /// <summary>
        /// Generates a puzzle from a seed and some base puzzle data.
        /// </summary>
        /// <param name="puzzleDataString">The base puzzle data to use.</param>
        /// <param name="seed">The seed to use.</param>
        /// <returns>The puzzle data generated.</returns>
        private byte[,] GeneratePuzzle(string puzzleDataString, int seed)
        {
            var random = new Random(seed);
            var puzzleDataStrings = puzzleDataString.Split(' ');
            var puzzleData = new byte[9, 9];
            for (var i = 0; i < 9; i++)
                for (var j = 0; j < 9; j++)
                {
                    puzzleData[i, j] = byte.Parse(puzzleDataStrings[i][j].ToString());
                }

            for (var i = random.Next(50, 1000); i > 0; i--)
                switch (random.Next(6))
                {
                    case 0:
                        PuzzleMainDiagTransformation(puzzleData);
                        break;
                    case 1:
                        PuzzleMinorDiagTransformation(puzzleData);
                        break;
                    case 2:
                        PuzzleMinorRowSwap(random, puzzleData);
                        break;
                    case 3:
                        PuzzleMinorColumnSwap(random, puzzleData);
                        break;
                    case 4:
                        PuzzleMajorRowSwap(random, puzzleData);
                        break;
                    case 5:
                        PuzzleMajorColumnSwap(random, puzzleData);
                        break;
                    default: throw new ArgumentOutOfRangeException(null, "The switch statement hasn't handled this value");
                }
            return puzzleData;
        }

        /// <summary>
        /// Swaps a random group of 3 rows (1,2,3 or 4,5,6 or 7,8,9) with another group of 3 rows in the puzzle.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMajorRowSwap(Random random, byte[,] puzzleData)
        {
            switch (random.Next(3))
            {
                case 0:
                    SwapRows(puzzleData, 0, 3);
                    SwapRows(puzzleData, 1, 4);
                    SwapRows(puzzleData, 2, 5);
                    break;
                case 1:
                    SwapRows(puzzleData, 0, 6);
                    SwapRows(puzzleData, 1, 7);
                    SwapRows(puzzleData, 2, 8);
                    break;
                case 2:
                    SwapRows(puzzleData, 3, 6);
                    SwapRows(puzzleData, 4, 7);
                    SwapRows(puzzleData, 5, 8);
                    break;
            }
        }

        /// <summary>
        /// Swaps a random group of 3 columns (1,2,3 or 4,5,6 or 7,8,9) with another group of 3 columns in the puzzle.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMajorColumnSwap(Random random, byte[,] puzzleData)
        {
            switch (random.Next(3))
            {
                case 0:
                    SwapColumns(puzzleData, 0, 3);
                    SwapColumns(puzzleData, 1, 4);
                    SwapColumns(puzzleData, 2, 5);
                    break;
                case 1:
                    SwapColumns(puzzleData, 0, 6);
                    SwapColumns(puzzleData, 1, 7);
                    SwapColumns(puzzleData, 2, 8);
                    break;
                case 2:
                    SwapColumns(puzzleData, 3, 6);
                    SwapColumns(puzzleData, 4, 7);
                    SwapColumns(puzzleData, 5, 8);
                    break;
            }
        }

        /// <summary>
        /// Swaps a random row with another row in the same group of 3.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMinorRowSwap(Random random, byte[,] puzzleData)
        {
            var r = random.Next(1, 10);
            var startRow = (r-1)/3*3;
            switch (r % 3)
            {
                case 0:
                    SwapRows(puzzleData, startRow, startRow + 1);
                    break;
                case 1:
                    SwapRows(puzzleData, startRow, startRow + 2);
                    break;
                case 2:
                    SwapRows(puzzleData, startRow + 1, startRow + 2);
                    break;
            }
        }

        /// <summary>
        /// Swaps a random column with another column in the same group of 3.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMinorColumnSwap(Random random, byte[,] puzzleData)
        {
            var r = random.Next(1, 10);
            var startColumn = (r - 1)/3*3;
            switch (r % 3)
            {
                case 0:
                    SwapColumns(puzzleData, startColumn, startColumn + 1);
                    break;
                case 1:
                    SwapColumns(puzzleData, startColumn, startColumn + 2);
                    break;
                case 2:
                    SwapColumns(puzzleData, startColumn + 1, startColumn + 2);
                    break;
            }
        }

        /// <summary>
        /// Swaps all cells over the main diagonal of the puzzle.
        /// Reflects the puzzle over the main diagonal
        /// </summary>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMainDiagTransformation(byte[,] puzzleData)
        {
            for (var i = 0; i < 9; i++)
            {
                for (var j = 8; j > i; j--)
                {
                    var temp = puzzleData[i, j];
                    puzzleData[i, j] = puzzleData[j, i];
                    puzzleData[j, i] = temp;
                }
            }
        }

        /// <summary>
        /// Swaps all cells over the minor diagonal of the puzzle.
        /// Reflects the puzzle over the minor diagonal
        /// </summary>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        private void PuzzleMinorDiagTransformation(byte[,] puzzleData)
        {
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; i + j < 8; j++)
                {
                    var temp = puzzleData[i, j];
                    puzzleData[i, j] = puzzleData[8 - j, 8 - i];
                    puzzleData[8 - j, 8 - i] = temp;
                }
            }
        }

        /// <summary>
        /// Swaps two given rows over in the puzzle.
        /// </summary>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        /// <param name="row1">The first row to swap.</param>
        /// <param name="row2">The second row to swap.</param>
        private void SwapRows(byte[,] puzzleData, int row1, int row2)
        {
            for (var i = 0; i < 9; i++)
            {
                var temp = puzzleData[row1, i];
                puzzleData[row1, i] = puzzleData[row2, i];
                puzzleData[row2, i] = temp;
            }
        }

        /// <summary>
        /// Swaps two given columns over.
        /// </summary>
        /// <param name="puzzleData">The puzzle data to perform the transformation on.</param>
        /// <param name="column1">The first column to swap.</param>
        /// <param name="column2">The second column to swap.</param>
        private void SwapColumns(byte[,] puzzleData, int column1, int column2)
        {
            for (var i = 0; i < 9; i++)
            {
                var temp = puzzleData[i, column1];
                puzzleData[i, column1] = puzzleData[i, column2];
                puzzleData[i, column2] = temp;
            }
        }
    }
}