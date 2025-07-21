using System;

namespace ChessEngine
{
    public class TestComprehensivePromotionRunner
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Comprehensive Promotion Integration Tests...");
            Console.WriteLine("=" + new string('=', 60));
            
            try
            {
                // Run all comprehensive integration tests
                TestComprehensivePromotionIntegration.RunAllTests();
                
                Console.WriteLine();
                Console.WriteLine("=" + new string('=', 60));
                Console.WriteLine("All comprehensive promotion integration tests completed!");
                Console.WriteLine("Check the output above for detailed results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test execution failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}