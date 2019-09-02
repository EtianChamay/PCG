using System;
using System.Drawing;

namespace PCG
{
    class Program
    {
        private static readonly int MaxWidth = GlobalConfiguration.MaxWidth;
        private static readonly int MaxHeight = GlobalConfiguration.MaxHeight;


        static void Main(string[] args)
        {
            var map = new RoomSpace(new Point(0,0), new Point(MaxWidth,MaxHeight));

            map.Split(11);
            map.GenerateRoom();

            var board = new int[MaxWidth,MaxHeight];

            map.Draw(board, 1,3);
            PrintBoard(board);
        }

        private static void PrintBoard(int[,] board)
        {
            for (var x = 0; x < MaxWidth; x++)
            {
                for (var y = 0; y < MaxHeight; y++)
                {
                    Console.Write(board[x, y]);
                }

                Console.WriteLine("");
            }
        }
    }
}
