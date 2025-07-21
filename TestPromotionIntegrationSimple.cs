using System;
using System.IO;

namespace ChessEngine
{
    public class TestPromotionIntegrationSimple
    {
        public static void RunTests()
        {
            Console.WriteLine("=== Running Simple Promotion Integration Tests ===");
            
            TestBasicPromotionIntegration();
            TestAIPromotionIntegration();
            
            Console.WriteLine("=== Simple Promotion Integration Tests Completed ===");
        }

        private static void TestBasicPromotionIntegration()
        {
            Console.WriteLine("Testing basic promotion integration in MovePiece...");
            
            try
            {
                ChessBoard board = new ChessBoard();
                
                // Use reflection to set up a promotion scenario
                var boardField = typeof(ChessBoard).GetField("board", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                string[,] boardArray = (string[,])boardField.GetValue(board);
                
                // Clear the board and set up promotion scenario
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        boardArray[i, j] = ".";
                
                // Set up kings and pawn ready to promote
                boardArray[7, 4] = "WK";
                boardArray[0, 4] = "BK";
                boardArray[1, 0] = "WP"; // White pawn ready to promote
                
                // Set turn to white
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                turnField.SetValue(board, "W");
                
                // Simulate user input for promotion choice
                var originalIn = Console.In;
                var input = new StringReader("Q\n");
                Console.SetIn(input);
                
                try
                {
                    // Execute the promotion move
                    board.MovePiece(1, 0, 0, 0);
                    
                    // Check if promotion occurred
                    string promotedPiece = boardArray[0, 0];
                    if (promotedPiece == "WQ")
                    {
                        Console.WriteLine("✓ Basic promotion integration works - pawn promoted to Queen");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Promotion failed - expected WQ, got {promotedPiece}");
                    }
                    
                    // Check if turn switched
                    string currentTurn = board.CurrentTurn();
                    if (currentTurn == "B")
                    {
                        Console.WriteLine("✓ Turn switched correctly after promotion");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Turn switch failed - expected B, got {currentTurn}");
                    }
                }
                finally
                {
                    Console.SetIn(originalIn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed with exception: {ex.Message}");
            }
        }

        private static void TestAIPromotionIntegration()
        {
            Console.WriteLine("Testing AI promotion integration in MovePieceAI...");
            
            try
            {
                ChessBoard board = new ChessBoard();
                
                // Use reflection to set up a promotion scenario for AI
                var boardField = typeof(ChessBoard).GetField("board", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                string[,] boardArray = (string[,])boardField.GetValue(board);
                
                // Clear the board and set up promotion scenario
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        boardArray[i, j] = ".";
                
                // Set up kings and pawn ready to promote
                boardArray[7, 4] = "WK";
                boardArray[0, 4] = "BK";
                boardArray[6, 0] = "BP"; // Black pawn ready to promote
                
                // Set turn to black
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                turnField.SetValue(board, "B");
                
                // Execute AI promotion move
                board.MovePieceAI(6, 0, 7, 0);
                
                // Check if promotion occurred
                string promotedPiece = boardArray[7, 0];
                if (promotedPiece == "BQ")
                {
                    Console.WriteLine("✓ AI promotion integration works - pawn automatically promoted to Queen");
                }
                else
                {
                    Console.WriteLine($"✗ AI promotion failed - expected BQ, got {promotedPiece}");
                }
                
                // Check if turn switched
                string currentTurn = board.CurrentTurn();
                if (currentTurn == "W")
                {
                    Console.WriteLine("✓ Turn switched correctly after AI promotion");
                }
                else
                {
                    Console.WriteLine($"✗ AI turn switch failed - expected W, got {currentTurn}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ AI test failed with exception: {ex.Message}");
            }
        }
    }
}