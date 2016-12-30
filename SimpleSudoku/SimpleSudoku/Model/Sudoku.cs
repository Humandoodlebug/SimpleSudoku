using System;
using Windows.Foundation.Metadata;

namespace SC.SimpleSudoku.Model
{
    internal class Sudoku
    {
        public byte[,] ProblemData { get; set; }

        public byte[,] SolutionData { get; set; }

        public int Seed { get; private set; }

        public Sudoku(Base_Puzzle basePuzzle, int? seed = null)
        {
            if (seed == null)
                seed = new Random().Next();
            Seed = seed.Value;
            ProblemData = GeneratePuzzle(basePuzzle.PuzzleProblemData, Seed);
            SolutionData = GeneratePuzzle(basePuzzle.PuzzleSolutionData, Seed);
        }

        //private byte[] GetRow([Range(0, 8)] int index)
        //{
        //    var row = new byte[9];
        //    for (var i = 0; i < row.Length; i++)
        //        row[i] = ProblemData[index, i];
        //    return row;
        //}

        //private byte[] GetColumn([Range(0, 8)] int index)
        //{
        //    var column = new byte[9];
        //    for (var i = 0; i < column.Length; i++)
        //        column[i] = ProblemData[i, index];
        //    return column;
        //}

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
                switch (random.Next(8))
                {
                    case 0:
                        PuzzleRowTransformation(puzzleData);
                        break;
                    case 1:
                        PuzzleColumnTransformation(puzzleData);
                        break;
                    case 2:
                        PuzzleMainDiagTransformation(puzzleData);
                        break;
                    case 3:
                        PuzzleMinorDiagTransformation(puzzleData);
                        break;
                    case 4:
                        PuzzleMinorRowSwap(random, puzzleData);
                        break;
                    case 5:
                        PuzzleMinorColumnSwap(random, puzzleData);
                        break;
                    case 6:
                        PuzzleMajorRowSwap(random, puzzleData);
                        break;
                    case 7:
                        PuzzleMajorColumnSwap(random, puzzleData);
                        break;
                    default: throw new ArgumentOutOfRangeException(null, "The switch statement hasn't handled this value");
                }
            return puzzleData;
        }

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

        private void PuzzleMinorRowSwap(Random random, byte[,] puzzleData)
        {
            var r = random.Next(1, 10);
            switch (r % 3)
            {
                case 0:
                    SwapRows(puzzleData, r / 3, r / 3 + 1);
                    break;
                case 1:
                    SwapRows(puzzleData, r / 3, r / 3 + 2);
                    break;
                case 2:
                    SwapRows(puzzleData, r / 3 + 1, r / 3 + 2);
                    break;
            }
        }

        private void PuzzleMinorColumnSwap(Random random, byte[,] puzzleData)
        {
            var r = random.Next(1, 10);
            switch (r % 3)
            {
                case 0:
                    SwapColumns(puzzleData, r / 3, r / 3 + 1);
                    break;
                case 1:
                    SwapColumns(puzzleData, r / 3, r / 3 + 2);
                    break;
                case 2:
                    SwapColumns(puzzleData, r / 3 + 1, r / 3 + 2);
                    break;
            }
        }

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

        private void PuzzleMinorDiagTransformation(byte[,] puzzleData)
        {
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; (i + j) < 8; j++)
                {
                    var temp = puzzleData[i, j];
                    puzzleData[i, j] = puzzleData[8 - i, 8 - j];
                    puzzleData[8 - i, 8 - j] = temp;
                }
            }
        }

        private void PuzzleRowTransformation(byte[,] puzzleData)
        {
            SwapRows(puzzleData, 0, 8);
            SwapRows(puzzleData, 1, 7);
            SwapRows(puzzleData, 2, 4);
        }

        private void SwapRows(byte[,] puzzleData, int row1, int row2)
        {
            for (var i = 0; i < 9; i++)
            {
                var temp = puzzleData[row1, i];
                puzzleData[row1, i] = puzzleData[row2, i];
                puzzleData[row2, i] = temp;
            }
        }

        private void PuzzleColumnTransformation(byte[,] puzzleData)
        {
            SwapRows(puzzleData, 0, 8);
            SwapRows(puzzleData, 1, 7);
            SwapRows(puzzleData, 2, 4);
        }

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