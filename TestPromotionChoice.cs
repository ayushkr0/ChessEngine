using System;
using System.Reflection;

namespace ChessEngine
{
    class TestPromotionChoice
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Piece Selection Interface...");
            PromotionTest.RunTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}