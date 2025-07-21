using System;
using System.IO;

namespace ChessEngine
{
    class VerifyPromotionIntegration
    {
        static void Main()
        {
            Console.WriteLine("=== Verifying Promotion Integration ===");
            
            // Test 1: Verify MovePiece handles promotion
            Console.WriteLine("Test 1: Human player promotion via MovePiece");
            TestHumanPromotion();
            
            // Test 2: Verify MovePieceAI handles promotion
            Console.WriteLine("\nTest 2: AI promotion via MovePieceAI");
            TestAIPromotion();
            
            Console.WriteLine("\n=== Verification Complete ===");
        }
        
        static void TestHumanPromotion()
        {
            try
            {
                ChessBoard board = new ChessBoard();
                
                // Use reflection to access private fields for testing
                var boardField = typeof(ChessBoard).GetField("board", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                string[,] boardArray = (string[,])boardField.GetValue(board);
                
                // Set up promotion scenario
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        boardArray[i, j] = ".";
                
                boardArray[7, 4] = "WK";  // White king
                boardArray[0, 4] = "BK";  // Black king  
                boardArray[1, 0] = "WP";  // White pawn ready to promote
                turnField.SetValue(board, "W");
                
                // Simulate user choosing Queen
                var originalIn = Console.In;
                var input = new StringReader("Q\n");
                Console.SetIn(input);
                
                try
                {
                    board.MovePiece(1, 0, 0, 0);
                    
                    string result = boardArray[0, 0];
                    if (result == "WQ")
                    {
                        Console.WriteLine("✓ PASS: Pawn promoted to Queen via MovePiece");
                    }
                    else
                    {
                        Console.WriteLine($"✗ FAIL: Expected WQ, got {result}");
                    }
                }
                finally
                {
                    Console.SetIn(originalIn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: Exception occurred - {ex.Message}");
            }
        }
        
        static void TestAIPromotion()
        {
            try
            {
                ChessBoard board = new ChessBoard();
                
                // Use reflection to access private fields for testing
                var boardField = typeof(ChessBoard).GetField("board", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                string[,] boardArray = (string[,])boardField.GetValue(board);
                
                // Set up promotion scenario
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        boardArray[i, j] = ".";
                
                boardArray[7, 4] = "WK";  // White king
                boardArray[0, 4] = "BK";  // Black king  
                boardArray[6, 0] = "BP";  // Black pawn ready to promote
                turnField.SetValue(board, "B");
                
                board.MovePieceAI(6, 0, 7, 0);
                
                string result = boardArray[7, 0];
                if (result == "BQ")
                {
                    Console.WriteLine("✓ PASS: AI pawn promoted to Queen via MovePieceAI");
                }
                else
                {
                    Console.WriteLine($"✗ FAIL: Expected BQ, got {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAIL: Exception occurred - {ex.Message}");
            }
        }
    }
}