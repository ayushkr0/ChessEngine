using System;

namespace ChessEngine
{
    class RunMultipleQueensTest
    {
        static void Main()
        {
            Console.WriteLine("=== Starting Multiple Queens Support Test ===");
            
            try
            {
                TestMultipleQueensSupport.RunAllTests();
                
                Console.WriteLine("\n=== All Multiple Queens Support Tests Completed Successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== Test Failed ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}