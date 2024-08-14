using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Utils
    {
        public static char[] MirrorSudoku(char[] sudoku)
        {
            char[] mirrored = new char[81];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    mirrored[i * 9 + j] = sudoku[i * 9 + (8 - j)];
                }
            }

            return mirrored;
        }

        public static char[] RotateSudoku(char[] sudoku)
        {
            char[] rotated = new char[81];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    rotated[j * 9 + (8 - i)] = sudoku[i * 9 + j];
                }
            }

            return rotated;
        }
    }
}
