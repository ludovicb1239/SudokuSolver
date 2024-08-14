using SudokuSolver;
using System.Diagnostics;
using System.Drawing;
namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void _1Easy()
        {
            List<char> grid =
            [
                .. "*8****213".ToCharArray(0, 9),
                .. "*********".ToCharArray(0, 9),
                .. "***31*56*".ToCharArray(0, 9),
                .. "721*983*5".ToCharArray(0, 9),
                .. "539*746*1".ToCharArray(0, 9),
                .. "8*4*****7".ToCharArray(0, 9),
                .. "*98******".ToCharArray(0, 9),
                .. "*569*7***".ToCharArray(0, 9),
                .. "*7**651*2".ToCharArray(0, 9),
            ];
            Board b = new Board(grid.ToArray());
            Console.Write(b.ToString());
            bool solved = b.Solve();
            Console.WriteLine(b.ToString());
            Assert.IsTrue(solved);

        }
        [Test]
        public void _2Medium()
        {
            char[] grid =
            {
                '2', '*', '*', '5', '*', '7', '4', '*', '6',
                '*', '*', '*', '*', '3', '1', '*', '*', '*',
                '*', '*', '*', '*', '*', '*', '2', '3', '*',
                '*', '*', '*', '*', '2', '*', '*', '*', '*',
                '8', '6', '*', '3', '1', '*', '*', '*', '*',
                '*', '4', '5', '*', '*', '*', '*', '*', '*',
                '*', '*', '9', '*', '*', '*', '7', '*', '*',
                '*', '*', '6', '9', '5', '*', '*', '*', '2',
                '*', '*', '1', '*', '*', '6', '*', '*', '8'
            };
            for(int i = 0; i < 4; i++)
            {
                TrySudoku(grid);
                grid = Utils.RotateSudoku(grid);
            }
            grid = Utils.MirrorSudoku(grid);
            for (int i = 0; i < 4; i++)
            {
                TrySudoku(grid);
                grid = Utils.RotateSudoku(grid);
            }

        }
        [Test]
        public void _3Hard()
        {
            char[] grid =
            {
                '*', '*', '6', '5', '*', '*', '*', '*', '*',
                '7', '*', '5', '*', '*', '2', '3', '*', '*',
                '*', '3', '*', '*', '*', '*', '*', '8', '*',
                '*', '5', '*', '*', '9', '6', '*', '7', '*',
                '1', '*', '4', '*', '*', '*', '*', '*', '8',
                '*', '*', '*', '8', '2', '*', '*', '*', '*',
                '*', '2', '*', '*', '*', '*', '*', '9', '*',
                '*', '*', '7', '2', '*', '*', '4', '*', '*',
                '*', '*', '*', '*', '*', '7', '5', '*', '*'
            };
            for (int i = 0; i < 4; i++)
            {
                TrySudoku(grid);
                grid = Utils.RotateSudoku(grid);
            }
            grid = Utils.MirrorSudoku(grid);
            for (int i = 0; i < 4; i++)
            {
                TrySudoku(grid);
                grid = Utils.RotateSudoku(grid);
            }

        }
        [Test]
        public void _4Test()
        {
            char[] grid = "8..........36......7..9.2...5...7.......457.....1...3...1....68..85...1..9....4..".ToCharArray();
            TrySudoku(grid);
        }
        void TrySudoku(char[] grid)
        {
            Board b = new Board(grid.ToArray());
            Console.Write(b.ToString());

            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool solved = b.Solve();
            sw.Stop();

            if (solved)
            {
                Console.WriteLine($"Solved in {sw.ElapsedMilliseconds} ms");
            }
            Console.WriteLine(b.ToString());
            Bitmap bmp = b.Draw();
            // Save the image to a file
            bmp.Save("sudoku.png");
            Assert.IsTrue(solved);
        }
    }
}