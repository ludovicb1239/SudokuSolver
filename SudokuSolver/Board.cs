using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Board
    {
        const char EMPTY = '.';
        int[] possibilities; //b0 = has no other poss, b1-b9 = could be possible
        int[] grid; //0 = is empty, 1 to 9
        readonly int[] colStart;
        readonly int[] rowStart;
        readonly int[] regStart;
        readonly List<int[]> linked;
        public Board(char[] grid)
        {
            if (grid.Length != 81)
                throw new Exception("Grid is the wrong size");

            this.grid = new int[grid.Length];
            this.possibilities = new int[grid.Length];
            for(int i = 0; i < grid.Length; i++)
            {
                if (grid[i] == EMPTY || grid[i] == '*')
                {
                    this.possibilities[i] = 0b0011_1111_1110;
                    this.grid[i] = 0;
                }
                else if (grid[i] > '0' && grid[i] <= '9')
                {
                    this.possibilities[i] = 1;
                    this.grid[i] = grid[i] - '0';
                }
                else
                    throw new Exception("Grid has unexpected characters");
            }

            colStart = new int[grid.Length];
            rowStart = new int[grid.Length];
            regStart = new int[grid.Length];
            for (int i = 0; i < grid.Length; i++)
            {
                colStart[i] = i % 9;
                rowStart[i] = (i / 9) * 9;
                regStart[i] = (colStart[i] / 3 * 3) + (rowStart[i] / 27 * 27);
            }
            linked = new();
            for (int pos = 0; pos < grid.Length; pos++)
            {
                List<int> arr = new();

                //Check column
                {
                    int start = colStart[pos];
                    for (int i = start; i < grid.Length; i += 9)
                    {
                        if (i == pos) continue;
                        if (!arr.Contains(i))
                            arr.Add(i);
                    }
                }

                //Check row
                {
                    int start = rowStart[pos];
                    for (int i = start; i < start + 9; i += 1)
                    {
                        if (i == pos) continue;
                        if (!arr.Contains(i))
                            arr.Add(i);
                    }
                }
                //Check square
                {
                    int start = regStart[pos];
                    for (int i = start; i < start + 3; i += 1)
                    {
                        if (i == pos) continue;
                        if (!arr.Contains(i))
                            arr.Add(i);
                    }
                    start += 9;
                    for (int i = start; i < start + 3; i += 1)
                    {
                        if (i == pos) continue;
                        if (!arr.Contains(i))
                            arr.Add(i);
                    }
                    start += 9;
                    for (int i = start; i < start + 3; i += 1)
                    {
                        if (i == pos) continue;
                        if (!arr.Contains(i))
                            arr.Add(i);
                    }
                }

                linked.Add(arr.ToArray());
            }
        }
        public bool Solve()
        {
            if (!Verify(false))
            {
                Console.WriteLine($"Double number");
                return false;
            }
            //First pass
            for (int pos = 0; pos < grid.Length; pos++)
            {
                if (!ReducePossibilities(pos))
                {
                    Console.WriteLine($"Could not solve : {pos}");
                    // A cell has no possibility
                    return false;
                }
            }
            Console.WriteLine("First pass done");
            if (!CheckConnection(0))
                return false;
            Console.WriteLine("Should be all done");
            return Verify(true);
        }
        bool CheckConnection(int pos)
        {
            if (pos == 81) return Done();
            if (grid[pos] != 0) return CheckConnection(pos + 1);

            int poss = possibilities[pos];
            int[] check = linked[pos]; //List of places to check
            for (int num = 1; num <= 9; num++)
            {
                if (((poss >> num) & 1) == 1)
                {
                    int choosenNum = num;
                    possibilities[pos] &= ~ (1 << choosenNum);
                    grid[pos] = choosenNum;
                    int[] oldGrid = grid.ToArray();
                    int[] oldPoss = possibilities.ToArray();

                    bool valid = true;
                    for (int i = 0; i < check.Length; i++)
                    {
                        if (!ReducePossibilities(check[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        if (CheckConnection(pos + 1))
                        {
                            return true;
                        }
                    }
                    grid = oldGrid;
                    possibilities = oldPoss;
                }
            }
            possibilities[pos] = poss;
            grid[pos] = 0;
            return false;
        }
        bool Done()
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (grid[i] == 0)
                    return false;
            }
            return Verify(true);
        }
        public bool Verify(bool isDone)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (grid[i] == 0)
                {
                    if (isDone) return false;
                    else continue;
                }
                if (!CorrectPLacement(i, grid[i]))
                    return false;
            }
            return true;
        }
        bool ReducePossibilities(int pos)
        {
            // Returns false if it has no possibilities
            if (grid[pos] != 0) return true;

            int[] check = linked[pos]; //List of places to check
            int poss = 0b0011_1111_1110;
            for (int i = 0; i < check.Length; i++)
            {
                if (grid[check[i]] != 0)
                {
                    poss &= ~(1 << grid[check[i]]); //Set to 0 the corresponding bit
                }
            }
            int total = 0;
            for (int num = 1; num <= 9; num++)
            {
                total += (poss >> num) & 1;
            }
            if (total == 0)
                return false;
            if (total == 1)
            {
                for (int num = 1; num <= 9; num++)
                {
                    if ((poss >> num) == 1)
                    {
                        poss = 1;
                        grid[pos] = num;
                        break;
                    }
                }

                for (int i = 0; i < check.Length; i++)
                {
                    if (!ReducePossibilities(check[i]))
                    {
                        return false;
                    }
                }
            }
            possibilities[pos] = poss;

            return true;
        }
        bool CorrectPLacement(int pos, int num)
        {
            // Returns true if it can place num at pos depending on grid

            int[] check = linked[pos]; //List of places to check
            for (int i = 0; i < check.Length; i++)
            {
                if (grid[check[i]] == num)
                {
                    return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            List<char> str = new();
            for (int i = 0; i < grid.Length; i++)
            {
                if (i % 9 == 0)
                    str.Add('\n');

                str.Add(grid[i] == 0 ? EMPTY : (char)((char)grid[i] + '0'));
            }
            str.Add('\n');
            return new string(str.ToArray());
        }
        public Bitmap Draw()
        {
            int cellSize = 40;
            int imageSize = cellSize * 9;
            Bitmap bitmap = new Bitmap(imageSize, imageSize);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Set background color to white
                graphics.Clear(Color.White);

                // Draw the grid
                Pen gridPen = new Pen(Color.Black, 2);

                // Draw the grid lines
                for (int i = 0; i <= 9; i++)
                {
                    int thickness = (i % 3 == 0) ? 4 : 2;
                    gridPen = new Pen(Color.Black, thickness);
                    graphics.DrawLine(gridPen, i * cellSize, 0, i * cellSize, imageSize);
                    graphics.DrawLine(gridPen, 0, i * cellSize, imageSize, i * cellSize);
                }

                // Draw the numbers in the grid
                Font font = new Font("Arial", 24, FontStyle.Bold);
                Brush textBrush = Brushes.Black;
                for (int i = 0; i < 81; i++)
                {
                    if (grid[i] != 0)
                    {
                        int row = i / 9;
                        int col = i % 9;
                        string text = ((char)((char)grid[i] + '0')).ToString();
                        SizeF textSize = graphics.MeasureString(text, font);
                        float x = col * cellSize + (cellSize - textSize.Width) / 2;
                        float y = row * cellSize + (cellSize - textSize.Height) / 2;
                        graphics.DrawString(text, font, textBrush, x, y);
                    }
                }
            }
            Console.WriteLine("Sudoku grid image created successfully!");
            return bitmap;
        }
    }
}
