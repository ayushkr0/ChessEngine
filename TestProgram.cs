using System;

namespace ChessEngine
{
    class TestProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running HandlePawnPromotion tests...");
            
            // Run HandlePawnPromotion tests
            TestHandlePawnPromotion handlePromotionTests = new TestHandlePawnPromotion();
            handlePromotionTests.RunAllTests();
            
            Console.WriteLine("Tests completed!");
        }
    }
}