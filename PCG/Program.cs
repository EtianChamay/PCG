using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PCG.DataStructures;

namespace PCG
{
    class Program
    {
        private const int MAX_WIDTH = 25;
        private const int MAX_HEIGHT = 50;


        static void Main(string[] args)
        {
            var map = new Room(new Point(0,0), new Point(MAX_WIDTH,MAX_HEIGHT));

            map.Split(4);

            var board = new int[MAX_WIDTH,MAX_HEIGHT];

            map.Draw(board, 1);
            PrintBoard(board);
        }

        private static void PrintBoard(int[,] board)
        {
            for (var x = 0; x < MAX_WIDTH; x++)
            {
                for (var y = 0; y < MAX_HEIGHT; y++)
                {
                    Console.Write(board[x, y]);
                }

                Console.WriteLine("");
            }
        }
    }
}
