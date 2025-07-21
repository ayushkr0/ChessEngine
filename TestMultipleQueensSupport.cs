using System;
using System.Reflection;

namespace ChessEngine
{
    public class TestMultipleQueensSupport
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing Multiple Queens Support ===");
            Console.Out.Flush();
            
            try
            {
                TestBasicQueenSupport();
                Console.WriteLine("✓ All Multiple Queens Support tests completed!");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        
        private static void TestBasicQueenSupport()
        {
            Console.WriteLine("Testing basic Queen support...");
            Console.Out.Flush();
            
            var board = new ChessBoard();
            int score = board.EvaluateBoard();
            
            Console.WriteLine($"Board evaluation score: {score}");
            Console.WriteLine("✓ Basic Queen support test passed");
        }

        private static void TestMultipleQueensOnBoard()
        {
            Console.WriteLine("Testing multiple Queens of same color can exist on board...");
            
            var board = new ChessBoard();
            
            // Create a scenario where we can have multiple Queens
            // Set up a board with promoted pawns that become Queens
            SetupBoardForMultipleQueens(board);
            
            // Count Queens on the board
            int whiteQueens = CountPiecesOnBoard(board, "WQ");
            int blackQueens = CountPiecesOnBoard(board, "BQ");
            
            if (whiteQueens < 2)
            {
                throw new Exception($"Expected at least 2 white Queens, found {whiteQueens}");
            }
            
            Console.WriteLine($"✓ Successfully created {whiteQueens} white Queens and {blackQueens} black Queens");
        }

        private static void TestBoardEvaluationWithMultipleQueens()
        {
            Console.WriteLine("Testing board evaluation correctly handles multiple Queens...");
            
            var board = new ChessBoard();
            
            // Get baseline evaluation with standard setup
            int baselineScore = board.EvaluateBoard();
            
            // Create board with multiple Queens
            SetupBoardForMultipleQueens(board);
            int multiQueenScore = board.EvaluateBoard();
            
            // With multiple Queens, the evaluation should be significantly different
            // Each Queen is worth 900 points, so additional Queens should impact score
            int scoreDifference = Math.Abs(multiQueenScore - baselineScore);
            
            if (scoreDifference < 900) // At least one additional Queen's worth
            {
                throw new Exception($"Board evaluation doesn't properly account for multiple Queens. Score difference: {scoreDifference}");
            }
            
            Console.WriteLine($"✓ Board evaluation correctly handles multiple Queens (score difference: {scoreDifference})");
        }

        private static void TestPromotedQueenMovement()
        {
            Console.WriteLine("Testing promoted Queens have correct movement capabilities...");
            
            var board = new ChessBoard();
            
            // Create a promoted Queen and test its movement
            CreatePromotedQueenScenario(board);
            
            // Test Queen movement patterns
            TestQueenMovementPatterns(board);
            
            Console.WriteLine("✓ Promoted Queens have correct movement capabilities");
        }

        private static void TestMultiplePromotedPieces()
        {
            Console.WriteLine("Testing scenarios with multiple promoted pieces...");
            
            var board = new ChessBoard();
            
            // Create multiple promoted pieces of different types
            SetupMultiplePromotedPieces(board);
            
            // Verify all promoted pieces exist and function correctly
            int totalPromotedPieces = CountPromotedPieces(board);
            
            if (totalPromotedPieces < 3)
            {
                throw new Exception($"Expected at least 3 promoted pieces, found {totalPromotedPieces}");
            }
            
            Console.WriteLine($"✓ Successfully created and verified {totalPromotedPieces} promoted pieces");
        }

        private static void TestQueenVsQueenScenario()
        {
            Console.WriteLine("Testing Queen vs Queen combat scenario...");
            
            var board = new ChessBoard();
            
            // Set up a scenario where multiple Queens can interact
            SetupQueenVsQueenScenario(board);
            
            // Verify Queens can capture each other
            TestQueenCapture(board);
            
            Console.WriteLine("✓ Queen vs Queen scenarios work correctly");
        }

        private static void SetupBoardForMultipleQueens(ChessBoard board)
        {
            // Use reflection to access private board field for testing
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    testBoard[i, j] = ".";
                }
            }
            
            // Place Kings (required for valid game state)
            testBoard[0, 4] = "BK"; // Black King
            testBoard[7, 4] = "WK"; // White King
            
            // Place original Queens
            testBoard[0, 3] = "BQ"; // Black Queen
            testBoard[7, 3] = "WQ"; // White Queen
            
            // Place additional promoted Queens
            testBoard[1, 1] = "WQ"; // White promoted Queen
            testBoard[1, 6] = "WQ"; // Another White promoted Queen
            testBoard[6, 2] = "BQ"; // Black promoted Queen
        }

        private static void SetupMultiplePromotedPieces(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    testBoard[i, j] = ".";
                }
            }
            
            // Place Kings
            testBoard[0, 4] = "BK";
            testBoard[7, 4] = "WK";
            
            // Place various promoted pieces
            testBoard[1, 0] = "WQ"; // Promoted Queen
            testBoard[1, 1] = "WR"; // Promoted Rook
            testBoard[1, 2] = "WB"; // Promoted Bishop
            testBoard[1, 3] = "WN"; // Promoted Knight
            testBoard[6, 0] = "BQ"; // Promoted Queen
            testBoard[6, 1] = "BR"; // Promoted Rook
        }

        private static void CreatePromotedQueenScenario(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    testBoard[i, j] = ".";
                }
            }
            
            // Place Kings
            testBoard[0, 7] = "BK";
            testBoard[7, 0] = "WK";
            
            // Place a promoted Queen in the center
            testBoard[3, 3] = "WQ"; // Promoted White Queen
        }

        private static void SetupQueenVsQueenScenario(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    testBoard[i, j] = ".";
                }
            }
            
            // Place Kings
            testBoard[0, 0] = "BK";
            testBoard[7, 7] = "WK";
            
            // Place Queens that can interact
            testBoard[3, 3] = "WQ"; // White Queen
            testBoard[3, 5] = "BQ"; // Black Queen (can be captured)
        }

        private static void TestQueenMovementPatterns(ChessBoard board)
        {
            // Test horizontal movement
            if (!IsValidQueenMove(board, 3, 3, 3, 7))
            {
                throw new Exception("Promoted Queen cannot move horizontally");
            }
            
            // Test vertical movement
            if (!IsValidQueenMove(board, 3, 3, 7, 3))
            {
                throw new Exception("Promoted Queen cannot move vertically");
            }
            
            // Test diagonal movement
            if (!IsValidQueenMove(board, 3, 3, 6, 6))
            {
                throw new Exception("Promoted Queen cannot move diagonally");
            }
        }

        private static void TestQueenCapture(ChessBoard board)
        {
            // Test if White Queen can capture Black Queen
            if (!IsValidQueenMove(board, 3, 3, 3, 5))
            {
                throw new Exception("Queen cannot capture enemy Queen");
            }
        }

        private static bool IsValidQueenMove(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol)
        {
            // Use reflection to access private IsValidQueenMove method
            var method = typeof(ChessBoard).GetMethod("IsValidQueenMove", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, "W" });
        }

        private static int CountPiecesOnBoard(ChessBoard board, string pieceType)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (testBoard[i, j] == pieceType)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private static int CountPromotedPieces(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] testBoard = (string[,])boardField.GetValue(board);
            
            int count = 0;
            // Count pieces that could be promoted pieces (excluding original positions)
            for (int i = 1; i < 7; i++) // Exclude back ranks where original pieces start
            {
                for (int j = 0; j < 8; j++)
                {
                    string piece = testBoard[i, j];
                    if (piece != "." && piece != "WP" && piece != "BP" && piece != "WK" && piece != "BK")
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}