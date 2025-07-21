using System;
using ChessEngine.Tests;

namespace ChessEngine
{
    class TestPromotionIntegrationRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Promotion Integration Tests...");
            Console.WriteLine();
            
            try
            {
                TestPromotionIntegration.RunAllTests();
                Console.WriteLine();
                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test execution failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}