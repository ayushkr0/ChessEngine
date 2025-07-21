using System;

namespace ChessEngine
{
    class TestBoardDisplay
    {
        static void Main()
        {
            Console.WriteLine("=== Testing Improved Chess Board Display ===");
            
            ChessBoard board = new ChessBoard();
            board.PrintBoard();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}