using System;
using System.Reflection;

namespace ChessEngine
{
    class VerifyMultipleQueens
    {
        static void Main()
        {
            Console.WriteLine("=== Verifying Multiple Queens Support ===\n");
            
            // Test 1: Verify board evaluation handles multiple Queens
            Console.WriteLine("Test 1: Board Evaluation with Multiple Queens");
            TestBoardEvaluation();
            
            // Test 2: Verify promoted piece creation
            Console.WriteLine("\nTest 2: Promoted Piece Creation");
            TestPromotedPieceCreation();
            
            // Test 3: Verify Queen movement validation
            Console.WriteLine("\nTest 3: Queen Movement Validation");
            TestQueenMovement();
            
            // Test 4: Verify promotion detection
            Console.WriteLine("\nTest 4: Promotion Detection");
            TestPromotionDetection();
            
            Console.WriteLine("\n=== All Multiple Queens Support Tests Completed ===");
        }
        
        static void TestBoardEvaluation()
        {
            var board = new ChessBoard();
            
            // Get baseline evaluation (should be close to 0 for equal material)
            int baselineScore = board.EvaluateBoard();
            Console.WriteLine($"Baseline evaluation: {baselineScore}");
            
            // Manually place additional Queens on the board using reflection
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Add extra Queens
            testBoard[2, 2] = "WQ"; // Extra White Queen
            testBoard[5, 5] = "BQ"; // Extra Black Queen
            
            int newScore = board.EvaluateBoard();
            Console.WriteLine($"Evaluation with extra Queens: {newScore}");
            
            // The score should be similar since we added equal material to both sides
            int scoreDifference = Math.Abs(newScore - baselineScore);
            Console.WriteLine($"Score difference: {scoreDifference}");
            
            if (scoreDifference < 100) // Allow some tolerance
            {
                Console.WriteLine("✓ Board evaluation correctly handles multiple Queens");
            }
            else
            {
                Console.WriteLine("✗ Board evaluation may have issues with multiple Queens");
            }
        }
        
        static void TestPromotedPieceCreation()
        {
            var board = new ChessBoard();
            
            // Test CreatePromotedPiece method using reflection
            var method = typeof(ChessBoard).GetMethod("CreatePromotedPiece", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test creating promoted Queens
            string whiteQueen = (string)method.Invoke(board, new object[] { "W", 'Q' });
            string blackQueen = (string)method.Invoke(board, new object[] { "B", 'Q' });
            
            Console.WriteLine($"White promoted Queen: {whiteQueen}");
            Console.WriteLine($"Black promoted Queen: {blackQueen}");
            
            if (whiteQueen == "WQ" && blackQueen == "BQ")
            {
                Console.WriteLine("✓ Promoted piece creation works correctly");
            }
            else
            {
                Console.WriteLine("✗ Promoted piece creation has issues");
            }
        }
        
        static void TestQueenMovement()
        {
            var board = new ChessBoard();
            
            // Test IsValidQueenMove method using reflection
            var method = typeof(ChessBoard).GetMethod("IsValidQueenMove", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Set up a board with a Queen in the center
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Clear the board and place a Queen
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    testBoard[i, j] = ".";
                }
            }
            testBoard[3, 3] = "WQ"; // White Queen in center
            testBoard[0, 0] = "BK"; // Black King (required)
            testBoard[7, 7] = "WK"; // White King (required)
            
            // Test Queen movement patterns
            bool horizontal = (bool)method.Invoke(board, new object[] { 3, 3, 3, 7, "W" });
            bool vertical = (bool)method.Invoke(board, new object[] { 3, 3, 7, 3, "W" });
            bool diagonal = (bool)method.Invoke(board, new object[] { 3, 3, 6, 6, "W" });
            
            Console.WriteLine($"Horizontal movement: {horizontal}");
            Console.WriteLine($"Vertical movement: {vertical}");
            Console.WriteLine($"Diagonal movement: {diagonal}");
            
            if (horizontal && vertical && diagonal)
            {
                Console.WriteLine("✓ Queen movement validation works correctly");
            }
            else
            {
                Console.WriteLine("✗ Queen movement validation has issues");
            }
        }
        
        static void TestPromotionDetection()
        {
            var board = new ChessBoard();
            
            // Test RequiresPromotion method using reflection
            var method = typeof(ChessBoard).GetMethod("RequiresPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test promotion detection
            bool whitePromotion = (bool)method.Invoke(board, new object[] { 0, "W" }); // White pawn to rank 8
            bool blackPromotion = (bool)method.Invoke(board, new object[] { 7, "B" }); // Black pawn to rank 1
            bool noPromotion = (bool)method.Invoke(board, new object[] { 4, "W" }); // White pawn to middle rank
            
            Console.WriteLine($"White pawn to rank 8: {whitePromotion}");
            Console.WriteLine($"Black pawn to rank 1: {blackPromotion}");
            Console.WriteLine($"White pawn to rank 4: {noPromotion}");
            
            if (whitePromotion && blackPromotion && !noPromotion)
            {
                Console.WriteLine("✓ Promotion detection works correctly");
            }
            else
            {
                Console.WriteLine("✗ Promotion detection has issues");
            }
        }
    }
}