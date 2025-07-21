using System;
using System.Reflection;

namespace ChessEngine
{
    class TestCreatePromotedPiece
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing CreatePromotedPiece method...");
            
            var board = new ChessBoard();
            var method = typeof(ChessBoard).GetMethod("CreatePromotedPiece", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test valid cases
            TestCase(board, method, "W", 'Q', "WQ", "White Queen");
            TestCase(board, method, "W", 'R', "WR", "White Rook");
            TestCase(board, method, "W", 'B', "WB", "White Bishop");
            TestCase(board, method, "W", 'N', "WN", "White Knight");
            
            TestCase(board, method, "B", 'Q', "BQ", "Black Queen");
            TestCase(board, method, "B", 'R', "BR", "Black Rook");
            TestCase(board, method, "B", 'B', "BB", "Black Bishop");
            TestCase(board, method, "B", 'N', "BN", "Black Knight");
            
            // Test case insensitive
            TestCase(board, method, "W", 'q', "WQ", "White Queen (lowercase)");
            TestCase(board, method, "B", 'r', "BR", "Black Rook (lowercase)");
            
            // Test invalid cases
            TestException(board, method, "X", 'Q', "Invalid color");
            TestException(board, method, "W", 'K', "Invalid piece type");
            
            Console.WriteLine("All tests completed!");
        }
        
        static void TestCase(ChessBoard board, MethodInfo method, string color, char pieceType, string expected, string testName)
        {
            try
            {
                string result = (string)method.Invoke(board, new object[] { color, pieceType });
                bool passed = result == expected;
                Console.WriteLine($"{testName}: {(passed ? "PASS" : "FAIL")} (Expected: {expected}, Got: {result})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{testName}: FAIL (Exception: {ex.Message})");
            }
        }
        
        static void TestException(ChessBoard board, MethodInfo method, string color, char pieceType, string testName)
        {
            try
            {
                string result = (string)method.Invoke(board, new object[] { color, pieceType });
                Console.WriteLine($"{testName}: FAIL (Expected exception but got: {result})");
            }
            catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException)
            {
                Console.WriteLine($"{testName}: PASS (Correctly threw ArgumentException)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{testName}: FAIL (Wrong exception: {ex.GetType().Name})");
            }
        }
    }
}