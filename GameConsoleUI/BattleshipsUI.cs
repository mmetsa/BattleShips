using System;
using GameBrain;

namespace GameConsoleUI
{
    public static class BattleshipsUI
    {

        public static void DrawBoard(CellState[,] board, bool drawShips)
        {
            // add plus 1, since this is 0 based. length 0 is returned as -1;
            var width = board.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y
            for (var i = 1; i <= width; i++)
            {
                if (i < 10)
                {
                    if (i == 1)
                    {
                        Console.Write("  ");
                    }
                    Console.Write("  " + i + "  ");

                }
                else
                {
                    Console.Write("  " + i );
                }
            }

            Console.WriteLine();
            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                if (colIndex == 0)
                {
                    Console.Write("  ");
                }
                Console.Write("+----");
                if (colIndex + 1 == width)
                {
                    Console.Write("+");
                }
            }
            Console.WriteLine();

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                if (rowIndex > 8)
                {
                    Console.Write(rowIndex + 1);
                }
                else
                {
                    Console.Write(rowIndex + 1 + " ");
                }
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"| {CellString(board[colIndex, rowIndex], drawShips)} ");
                    if (colIndex + 1 == width)
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    if (colIndex == 0)
                    {
                        Console.Write("  ");
                    }
                    Console.Write("+----");
                    if (colIndex + 1 == width)
                    {
                        Console.Write("+");
                    }
                }
                Console.WriteLine();
                
            }
        }
        
        public static string CellString(CellState cellState, bool drawShips)
        {
            if (cellState.ShipId == null) return cellState.Bomb ? " *" : "  ";
            if (cellState.Bomb)
            {
                return "xx";
            }

            if (!drawShips)
            {
                return "  ";
            }

            if (cellState.ShipId < 10) 
            {
                return cellState.ShipId + " ";

            }
            return cellState.ShipId.ToString();
        }
    }
}