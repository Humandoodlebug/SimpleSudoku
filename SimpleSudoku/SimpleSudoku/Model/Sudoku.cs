using Windows.Foundation.Metadata;

namespace SC.SimpleSudoku.Model
{
    internal class Sudoku
    {
        public int[,] SudokuData { get; set; }

        private int[] GetRow([Range(0, 8)] int index)
        {
            var row = new int[9];
            for (var i = 0; i < row.Length; i++)
                row[i] = SudokuData[index, i];
            return row;
        }

        private int[] GetColumn([Range(0, 8)] int index)
        {
            var column = new int[9];
            for (var i = 0; i < column.Length; i++)
                column[i] = SudokuData[i, index];
            return column;
        }
    }
}