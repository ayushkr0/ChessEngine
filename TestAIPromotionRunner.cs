using System;

namespace ChessEngine
{
    class TestAIPromotionRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running AI Promotion Tests...\n");
            
            try
            {
                TestAIPromotion.RunAllTests();
                Console.WriteLine("\n✓ All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Test execution failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}