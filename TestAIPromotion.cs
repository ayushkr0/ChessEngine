using System;
using System.IO;

namespace ChessEngine
{
    public class TestAIPromotion
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== AI Promotion Tests ===");
            
            TestAIPromotionInMakeBestMove();
            TestAIPromotionInMinimaxSearch();
            TestAIPromotionDuringGameplay();
            TestAIPromotionEvaluation();
            
            Console.WriteLine("=== All AI Promotion Tests Completed ===");
        }

        private static void TestAIPromotionInMakeBestMove()
        {
            Console.WriteLine("\n--- Test: AI Promotion in MakeBestAIMove ---");
            
            var board = new ChessBoard();
            
            // Set up a position where AI (Black) can promote
            // Clear the board first
            ClearBoard(board);
            
            // Place black pawn on 2nd rank ready to promote
            SetPiece(board, 1, 0, "BP"); // Black pawn on a2
            SetPiece(board, 7, 4, "WK"); // White king
            SetPiece(board, 0, 4, "BK"); // Black king
            
            // Set turn to Black so AI can move
            SetCurrentTurn(board, "B");
            
            Console.WriteLine("Before AI move:");
            board.PrintBoard();
            
            // AI should promote the pawn
            board.MakeBestAIMove(2);
            
            Console.WriteLine("After AI move:");
            board.PrintBoard();
            
            // Verify promotion occurred - should be a Queen on rank 1
            string promotedPiece = GetPiece(board, 0, 0);
            if (promotedPiece == "BQ")
            {
                Console.WriteLine("✓ PASS: AI correctly promoted pawn to Queen");
            }
            else
            {
                Console.WriteLine($"✗ FAIL: Expected BQ, got {promotedPiece}");
            }
        }

        private static void TestAIPromotionInMinimaxSearch()
        {
            Console.WriteLine("\n--- Test: AI Promotion in Minimax Search ---");
            
            var board = new ChessBoard();
            
            // Set up position where promotion is the best move
            ClearBoard(board);
            
            // White pawn ready to promote and deliver check
            SetPiece(board, 1, 4, "WP"); // White pawn on e2 (ready to promote to e1)
            SetPiece(board, 0, 0, "BK"); // Black king on a1
            SetPiece(board, 7, 7, "WK"); // White king on h1
            
            SetCurrentTurn(board, "W");
            
            Console.WriteLine("Position for minimax evaluation:");
            board.PrintBoard();
            
            // AI should recognize promotion as best move
            board.MakeBestAIMove(3);
            
            Console.WriteLine("After AI evaluation and move:");
            board.PrintBoard();
            
            // Check if promotion occurred
            bool promotionFound = false;
            for (int col = 0; col < 8; col++)
            {
                string piece = GetPiece(board, 0, col);
                if (piece == "WQ")
                {
                    promotionFound = true;
                    break;
                }
            }
            
            if (promotionFound)
            {
                Console.WriteLine("✓ PASS: AI correctly evaluated promotion in minimax search");
            }
            else
            {
                Console.WriteLine("✗ FAIL: AI did not promote pawn in minimax search");
            }
        }

        private static void TestAIPromotionDuringGameplay()
        {
            Console.WriteLine("\n--- Test: AI Promotion During Gameplay ---");
            
            var board = new ChessBoard();
            
            // Simulate a game scenario where AI promotes
            ClearBoard(board);
            
            // Set up endgame position with pawn promotion opportunity
            SetPiece(board, 1, 3, "BP"); // Black pawn on d2
            SetPiece(board, 6, 0, "WP"); // White pawn on a7
            SetPiece(board, 7, 4, "WK"); // White king
            SetPiece(board, 0, 4, "BK"); // Black king
            
            SetCurrentTurn(board, "B");
            
            Console.WriteLine("Game position before AI move:");
            board.PrintBoard();
            
            // Capture console output to verify promotion message
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // AI makes move
            board.MakeBestAIMove(2);
            
            // Restore console output
            Console.SetOut(originalOut);
            string output = stringWriter.ToString();
            
            Console.WriteLine("After AI move:");
            board.PrintBoard();
            
            // Verify promotion message was displayed
            bool promotionMessageFound = output.Contains("AI promoted pawn to Queen!");
            
            // Verify Queen is on the board
            string promotedPiece = GetPiece(board, 0, 3);
            bool queenPlaced = promotedPiece == "BQ";
            
            if (promotionMessageFound && queenPlaced)
            {
                Console.WriteLine("✓ PASS: AI promotion during gameplay works correctly");
                Console.WriteLine($"  - Promotion message displayed: {promotionMessageFound}");
                Console.WriteLine($"  - Queen placed correctly: {queenPlaced}");
            }
            else
            {
                Console.WriteLine("✗ FAIL: AI promotion during gameplay failed");
                Console.WriteLine($"  - Promotion message displayed: {promotionMessageFound}");
                Console.WriteLine($"  - Queen placed correctly: {queenPlaced}");
                Console.WriteLine($"  - Piece at promotion square: {promotedPiece}");
            }
        }

        private static void TestAIPromotionEvaluation()
        {
            Console.WriteLine("\n--- Test: AI Promotion Evaluation ---");
            
            var board1 = new ChessBoard();
            var board2 = new ChessBoard();
            
            // Set up identical positions, one with pawn, one with promoted Queen
            ClearBoard(board1);
            ClearBoard(board2);
            
            // Board 1: Pawn about to promote
            SetPiece(board1, 1, 0, "WP");
            SetPiece(board1, 7, 4, "WK");
            SetPiece(board1, 0, 4, "BK");
            
            // Board 2: Already promoted Queen
            SetPiece(board2, 0, 0, "WQ");
            SetPiece(board2, 7, 4, "WK");
            SetPiece(board2, 0, 4, "BK");
            
            int score1 = board1.EvaluateBoard();
            int score2 = board2.EvaluateBoard();
            
            Console.WriteLine($"Score with pawn: {score1}");
            Console.WriteLine($"Score with Queen: {score2}");
            
            // Queen should be significantly more valuable than pawn
            int scoreDifference = score2 - score1;
            int expectedDifference = 900 - 100; // Queen (900) - Pawn (100) = 800
            
            if (scoreDifference == expectedDifference)
            {
                Console.WriteLine($"✓ PASS: AI correctly evaluates promotion value (difference: {scoreDifference})");
            }
            else
            {
                Console.WriteLine($"✗ FAIL: AI evaluation incorrect (expected: {expectedDifference}, got: {scoreDifference})");
            }
        }

        // Helper methods using reflection to access private board state
        private static void ClearBoard(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var boardArray = (string[,])boardField.GetValue(board);
            
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    boardArray[row, col] = ".";
                }
            }
        }

        private static void SetPiece(ChessBoard board, int row, int col, string piece)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var boardArray = (string[,])boardField.GetValue(board);
            boardArray[row, col] = piece;
        }

        private static string GetPiece(ChessBoard board, int row, int col)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var boardArray = (string[,])boardField.GetValue(board);
            return boardArray[row, col];
        }

        private static void SetCurrentTurn(ChessBoard board, string turn)
        {
            var turnField = typeof(ChessBoard).GetField("currentTurn", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            turnField.SetValue(board, turn);
        }
    }
}