using System;
using System.IO;
using ChessEngine;

namespace ChessEngine.Tests
{
    public class TestPromotionIntegration
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Running Promotion Integration Tests ===");
            
            TestHumanPlayerPromotionFlow();
            TestAIPlayerPromotionFlow();
            TestPromotionAfterMoveValidation();
            TestPromotionBeforeTurnSwitch();
            TestAtomicBoardStateUpdate();
            TestPromotionDuringNormalGameplay();
            
            Console.WriteLine("=== All Promotion Integration Tests Completed ===");
        }

        private static void TestHumanPlayerPromotionFlow()
        {
            Console.WriteLine("Testing human player promotion flow...");
            
            ChessBoard board = new ChessBoard();
            
            // Set up a scenario where white pawn can promote
            // Move white pawn to 7th rank (row 1)
            SetupPromotionScenario(board, "W");
            
            // Simulate the move that should trigger promotion
            // We'll need to redirect console input for this test
            var originalIn = Console.In;
            var input = new StringReader("Q\n"); // Choose Queen
            Console.SetIn(input);
            
            try
            {
                // Move pawn from row 1 to row 0 (promotion rank)
                board.MovePiece(1, 0, 0, 0);
                
                // Verify the pawn was promoted to Queen
                string piece = GetPieceAt(board, 0, 0);
                if (piece == "WQ")
                {
                    Console.WriteLine("✓ Human player promotion flow works correctly");
                }
                else
                {
                    Console.WriteLine($"✗ Expected WQ, got {piece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestAIPlayerPromotionFlow()
        {
            Console.WriteLine("Testing AI player promotion flow...");
            
            ChessBoard board = new ChessBoard();
            
            // Set up a scenario where black pawn can promote via AI
            SetupPromotionScenario(board, "B");
            
            // Use MovePieceAI to simulate AI promotion
            board.MovePieceAI(6, 0, 7, 0);
            
            // Verify the pawn was automatically promoted to Queen
            string piece = GetPieceAt(board, 7, 0);
            if (piece == "BQ")
            {
                Console.WriteLine("✓ AI player promotion flow works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Expected BQ, got {piece}");
            }
        }

        private static void TestPromotionAfterMoveValidation()
        {
            Console.WriteLine("Testing promotion occurs after move validation...");
            
            ChessBoard board = new ChessBoard();
            
            // Set up invalid promotion scenario (pawn blocked)
            SetupBlockedPromotionScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("Q\n");
            Console.SetIn(input);
            
            try
            {
                // Try to move pawn to blocked square - should fail validation
                board.MovePiece(1, 0, 0, 0);
                
                // Verify pawn is still in original position (move was invalid)
                string piece = GetPieceAt(board, 1, 0);
                if (piece == "WP")
                {
                    Console.WriteLine("✓ Promotion correctly occurs only after move validation");
                }
                else
                {
                    Console.WriteLine($"✗ Expected WP at original position, got {piece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotionBeforeTurnSwitch()
        {
            Console.WriteLine("Testing promotion occurs before turn switch...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionScenario(board, "W");
            
            var originalIn = Console.In;
            var input = new StringReader("R\n"); // Choose Rook
            Console.SetIn(input);
            
            try
            {
                string turnBefore = board.CurrentTurn();
                board.MovePiece(1, 0, 0, 0);
                string turnAfter = board.CurrentTurn();
                
                // Verify promotion happened and turn switched
                string piece = GetPieceAt(board, 0, 0);
                if (piece == "WR" && turnBefore == "W" && turnAfter == "B")
                {
                    Console.WriteLine("✓ Promotion occurs before turn switch");
                }
                else
                {
                    Console.WriteLine($"✗ Promotion/turn sequence failed. Piece: {piece}, Turn before: {turnBefore}, Turn after: {turnAfter}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestAtomicBoardStateUpdate()
        {
            Console.WriteLine("Testing atomic board state update...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionScenario(board, "W");
            
            var originalIn = Console.In;
            var input = new StringReader("B\n"); // Choose Bishop
            Console.SetIn(input);
            
            try
            {
                // Verify original state
                string originalPiece = GetPieceAt(board, 1, 0);
                string targetSquare = GetPieceAt(board, 0, 0);
                
                board.MovePiece(1, 0, 0, 0);
                
                // Verify atomic update: original square empty, target has promoted piece
                string newOriginal = GetPieceAt(board, 1, 0);
                string newTarget = GetPieceAt(board, 0, 0);
                
                if (originalPiece == "WP" && targetSquare == "." && 
                    newOriginal == "." && newTarget == "WB")
                {
                    Console.WriteLine("✓ Board state updated atomically");
                }
                else
                {
                    Console.WriteLine($"✗ Atomic update failed. Original: {originalPiece}->{newOriginal}, Target: {targetSquare}->{newTarget}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotionDuringNormalGameplay()
        {
            Console.WriteLine("Testing promotion during normal gameplay scenario...");
            
            ChessBoard board = new ChessBoard();
            
            // Simulate a realistic game scenario leading to promotion
            SimulateGameToPromotion(board);
            
            var originalIn = Console.In;
            var input = new StringReader("N\n"); // Choose Knight
            Console.SetIn(input);
            
            try
            {
                // Execute the promotion move
                board.MovePiece(1, 4, 0, 4);
                
                // Verify promotion worked in gameplay context
                string piece = GetPieceAt(board, 0, 4);
                if (piece == "WN")
                {
                    Console.WriteLine("✓ Promotion works correctly during normal gameplay");
                }
                else
                {
                    Console.WriteLine($"✗ Expected WN, got {piece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        // Helper methods
        private static void SetupPromotionScenario(ChessBoard board, string color)
        {
            // Use reflection to access private board field for test setup
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings (required for valid game state)
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            if (color == "W")
            {
                // Place white pawn ready to promote (row 1, one move from promotion)
                boardArray[1, 0] = "WP";
                // Set current turn to white
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                turnField.SetValue(board, "W");
            }
            else
            {
                // Place black pawn ready to promote (row 6, one move from promotion)
                boardArray[6, 0] = "BP";
                // Set current turn to black
                var turnField = typeof(ChessBoard).GetField("currentTurn", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                turnField.SetValue(board, "B");
            }
        }

        private static void SetupBlockedPromotionScenario(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // Place white pawn ready to promote but block the target square
            boardArray[1, 0] = "WP";
            boardArray[0, 0] = "WR"; // Block with own piece
            
            var turnField = typeof(ChessBoard).GetField("currentTurn", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            turnField.SetValue(board, "W");
        }

        private static void SimulateGameToPromotion(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Clear the board
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up a realistic endgame scenario
            boardArray[7, 4] = "WK";  // White king
            boardArray[0, 4] = "BK";  // Black king
            boardArray[1, 4] = "WP";  // White pawn about to promote
            boardArray[2, 3] = "BR";  // Black rook
            
            var turnField = typeof(ChessBoard).GetField("currentTurn", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            turnField.SetValue(board, "W");
        }

        private static string GetPieceAt(ChessBoard board, int row, int col)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            return boardArray[row, col];
        }
    }
}