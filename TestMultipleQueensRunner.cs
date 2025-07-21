using System;

namespace ChessEngine
{
    class TestMultipleQueensRunner
    {
        static void Main(string[] args)
        {
            try
            {
                TestMultipleQueensSupport.RunAllTests();
                
                Console.WriteLine("\n=== All Multiple Queens Support Tests Completed Successfully! ===");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== Test Failed ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}