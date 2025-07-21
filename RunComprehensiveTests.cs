using System;

namespace ChessEngine
{
    class RunComprehensiveTests
    {
        static void Main()
        {
            Console.WriteLine("=== Starting Comprehensive Promotion Integration Tests ===");
            Console.WriteLine();
            
            try
            {
                TestComprehensivePromotionIntegration.RunAllTests();
                
                Console.WriteLine();
                Console.WriteLine("=== All Tests Completed Successfully ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test execution failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}