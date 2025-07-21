using System;
using System.Reflection;

namespace ChessEngine
{
    class TestRunner
    {
        static void MainTest(string[] args)
        {
            // Run the comprehensive promotion tests
            PromotionTest.RunTests();
            
            // Run HandlePawnPromotion tests
            TestHandlePawnPromotion handlePromotionTests = new TestHandlePawnPromotion();
            handlePromotionTests.RunAllTests();
        }
    }
}