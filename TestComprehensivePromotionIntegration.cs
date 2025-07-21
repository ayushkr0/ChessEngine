using System;
using System.IO;
using System.Reflection;

namespace ChessEngine
{
    public class TestComprehensivePromotionIntegration
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Running Comprehensive Promotion Integration Tests ===");
            
            TestEndToEndPromotionScenarios();
            TestPromotionWhileInCheck();
            TestPromotionDeliversCheckmate();
            TestGameFlowContinuesAfterPromotion();
            TestMultiplePromotionsInGame();
            TestPromotionWithCapture();
            TestAIPromotionInComplexPosition();
            
            Console.WriteLine("=== All Comprehensive Promotion Integration Tests Completed ===");
        }

        private static void TestEndToEndPromotionScenarios()
        {
            Console.WriteLine("Testing end-to-end promotion scenarios...");
            
            // Test 1: Complete human promotion flow from start to finish
            TestCompleteHumanPromotionFlow();
            
            // Test 2: Complete AI promotion flow from start to finish
            TestCompleteAIPromotionFlow();
            
            // Test 3: Promotion to different piece types
            TestPromotionToAllPieceTypes();
        }

        private static void TestCompleteHumanPromotionFlow()
        {
            Console.WriteLine("  Testing complete human promotion flow...");
            
            ChessBoard board = new ChessBoard();
            SetupEndgamePromotionScenario(board, "W");
            
            var originalIn = Console.In;
            var input = new StringReader("R\n"); // Choose Rook
            Console.SetIn(input);
            
            try
            {
                // Capture initial state
                string initialTurn = board.CurrentTurn();
                string pawnPosition = GetPieceAt(board, 1, 3);
                
                // Execute promotion move
                board.MovePiece(1, 3, 0, 3);
                
                // Verify complete flow
                string promotedPiece = GetPieceAt(board, 0, 3);
                string newTurn = board.CurrentTurn();
                string originalSquare = GetPieceAt(board, 1, 3);
                
                if (initialTurn == "W" && pawnPosition == "WP" && 
                    promotedPiece == "WR" && newTurn == "B" && originalSquare == ".")
                {
                    Console.WriteLine("    ✓ Complete human promotion flow successful");
                }
                else
                {
                    Console.WriteLine($"    ✗ Human promotion flow failed. Initial: {initialTurn}/{pawnPosition}, Final: {promotedPiece}/{newTurn}/{originalSquare}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestCompleteAIPromotionFlow()
        {
            Console.WriteLine("  Testing complete AI promotion flow...");
            
            ChessBoard board = new ChessBoard();
            SetupEndgamePromotionScenario(board, "B");
            
            // Capture initial state
            string initialTurn = board.CurrentTurn();
            string pawnPosition = GetPieceAt(board, 6, 3);
            
            // Execute AI promotion move
            board.MovePieceAI(6, 3, 7, 3);
            
            // Verify complete flow
            string promotedPiece = GetPieceAt(board, 7, 3);
            string newTurn = board.CurrentTurn();
            string originalSquare = GetPieceAt(board, 6, 3);
            
            if (initialTurn == "B" && pawnPosition == "BP" && 
                promotedPiece == "BQ" && newTurn == "W" && originalSquare == ".")
            {
                Console.WriteLine("    ✓ Complete AI promotion flow successful");
            }
            else
            {
                Console.WriteLine($"    ✗ AI promotion flow failed. Initial: {initialTurn}/{pawnPosition}, Final: {promotedPiece}/{newTurn}/{originalSquare}");
            }
        }

        private static void TestPromotionToAllPieceTypes()
        {
            Console.WriteLine("  Testing promotion to all piece types...");
            
            string[] pieceTypes = { "Q", "R", "B", "N" };
            string[] pieceNames = { "Queen", "Rook", "Bishop", "Knight" };
            
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                ChessBoard board = new ChessBoard();
                SetupEndgamePromotionScenario(board, "W");
                
                var originalIn = Console.In;
                var input = new StringReader($"{pieceTypes[i]}\n");
                Console.SetIn(input);
                
                try
                {
                    board.MovePiece(1, 3, 0, 3);
                    string promotedPiece = GetPieceAt(board, 0, 3);
                    
                    if (promotedPiece == $"W{pieceTypes[i]}")
                    {
                        Console.WriteLine($"    ✓ Promotion to {pieceNames[i]} successful");
                    }
                    else
                    {
                        Console.WriteLine($"    ✗ Promotion to {pieceNames[i]} failed - got {promotedPiece}");
                    }
                }
                finally
                {
                    Console.SetIn(originalIn);
                }
            }
        }

        private static void TestPromotionWhileInCheck()
        {
            Console.WriteLine("Testing promotion while in check situations...");
            
            // Test 1: Promotion move gets out of check
            TestPromotionGetsOutOfCheck();
            
            // Test 2: Promotion move while in check (blocking)
            TestPromotionBlocksCheck();
        }

        private static void TestPromotionGetsOutOfCheck()
        {
            Console.WriteLine("  Testing promotion that gets out of check...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionEscapesCheckScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("Q\n"); // Promote to Queen
            Console.SetIn(input);
            
            try
            {
                // Verify king is in check initially
                bool initialCheck = board.IsKingInCheck("W");
                
                // Execute promotion move that should get out of check
                board.MovePiece(1, 4, 0, 4);
                
                // Verify king is no longer in check
                bool finalCheck = board.IsKingInCheck("B"); // Turn switched to Black
                string promotedPiece = GetPieceAt(board, 0, 4);
                
                if (initialCheck && !finalCheck && promotedPiece == "WQ")
                {
                    Console.WriteLine("    ✓ Promotion successfully gets out of check");
                }
                else
                {
                    Console.WriteLine($"    ✗ Promotion escape check failed. Initial check: {initialCheck}, Final check: {finalCheck}, Piece: {promotedPiece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotionBlocksCheck()
        {
            Console.WriteLine("  Testing promotion that blocks check...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionBlocksCheckScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("B\n"); // Promote to Bishop to block
            Console.SetIn(input);
            
            try
            {
                // Verify king is in check initially
                bool initialCheck = board.IsKingInCheck("W");
                
                // Execute promotion move that blocks check
                board.MovePiece(1, 2, 0, 2);
                
                // Verify check is blocked
                bool finalCheck = board.IsKingInCheck("B"); // Turn switched
                string promotedPiece = GetPieceAt(board, 0, 2);
                
                if (initialCheck && !finalCheck && promotedPiece == "WB")
                {
                    Console.WriteLine("    ✓ Promotion successfully blocks check");
                }
                else
                {
                    Console.WriteLine($"    ✗ Promotion block check failed. Initial check: {initialCheck}, Final check: {finalCheck}, Piece: {promotedPiece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotionDeliversCheckmate()
        {
            Console.WriteLine("Testing promotion that delivers checkmate...");
            
            // Test 1: Promotion delivers immediate checkmate
            TestPromotionImmediateCheckmate();
            
            // Test 2: AI promotion delivers checkmate
            TestAIPromotionCheckmate();
        }

        private static void TestPromotionImmediateCheckmate()
        {
            Console.WriteLine("  Testing promotion delivers immediate checkmate...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionCheckmateScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("Q\n"); // Promote to Queen for checkmate
            Console.SetIn(input);
            
            try
            {
                // Verify no checkmate initially
                bool initialCheckmate = board.IsCheckmate("B");
                
                // Execute promotion move that should deliver checkmate
                board.MovePiece(1, 7, 0, 7);
                
                // Verify checkmate is delivered
                bool finalCheckmate = board.IsCheckmate("B");
                string promotedPiece = GetPieceAt(board, 0, 7);
                
                if (!initialCheckmate && finalCheckmate && promotedPiece == "WQ")
                {
                    Console.WriteLine("    ✓ Promotion successfully delivers checkmate");
                }
                else
                {
                    Console.WriteLine($"    ✗ Promotion checkmate failed. Initial mate: {initialCheckmate}, Final mate: {finalCheckmate}, Piece: {promotedPiece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestAIPromotionCheckmate()
        {
            Console.WriteLine("  Testing AI promotion delivers checkmate...");
            
            ChessBoard board = new ChessBoard();
            SetupAIPromotionCheckmateScenario(board);
            
            // Verify no checkmate initially
            bool initialCheckmate = board.IsCheckmate("W");
            
            // Execute AI promotion move that should deliver checkmate
            board.MovePieceAI(6, 7, 7, 7);
            
            // Verify checkmate is delivered
            bool finalCheckmate = board.IsCheckmate("W");
            string promotedPiece = GetPieceAt(board, 7, 7);
            
            if (!initialCheckmate && finalCheckmate && promotedPiece == "BQ")
            {
                Console.WriteLine("    ✓ AI promotion successfully delivers checkmate");
            }
            else
            {
                Console.WriteLine($"    ✗ AI promotion checkmate failed. Initial mate: {initialCheckmate}, Final mate: {finalCheckmate}, Piece: {promotedPiece}");
            }
        }

        private static void TestGameFlowContinuesAfterPromotion()
        {
            Console.WriteLine("Testing game flow continues correctly after promotion...");
            
            // Test 1: Normal moves possible after promotion
            TestMovesAfterPromotion();
            
            // Test 2: Promoted piece can be used immediately
            TestPromotedPieceUsage();
            
            // Test 3: Check detection works with promoted pieces
            TestCheckDetectionWithPromotedPieces();
        }

        private static void TestMovesAfterPromotion()
        {
            Console.WriteLine("  Testing normal moves possible after promotion...");
            
            ChessBoard board = new ChessBoard();
            SetupPostPromotionScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("N\n"); // Promote to Knight
            Console.SetIn(input);
            
            try
            {
                // Execute promotion
                board.MovePiece(1, 0, 0, 0);
                
                // Verify turn switched to Black
                string currentTurn = board.CurrentTurn();
                
                // Try to make a normal Black move
                board.MovePiece(1, 1, 2, 1); // Move black pawn
                
                // Verify move was successful and turn switched back
                string newTurn = board.CurrentTurn();
                string movedPiece = GetPieceAt(board, 2, 1);
                
                if (currentTurn == "B" && newTurn == "W" && movedPiece == "BP")
                {
                    Console.WriteLine("    ✓ Normal game flow continues after promotion");
                }
                else
                {
                    Console.WriteLine($"    ✗ Game flow disrupted. Turn sequence: W->B({currentTurn})->W({newTurn}), Moved piece: {movedPiece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotedPieceUsage()
        {
            Console.WriteLine("  Testing promoted piece can be used immediately...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotedPieceUsageScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("R\n"); // Promote to Rook
            Console.SetIn(input);
            
            try
            {
                // Execute promotion
                board.MovePiece(1, 0, 0, 0);
                
                // Skip Black's turn by making a dummy move
                board.MovePiece(6, 1, 5, 1);
                
                // Now try to use the promoted Rook
                board.MovePiece(0, 0, 0, 7); // Rook move across rank
                
                // Verify the promoted Rook moved successfully
                string rookPosition = GetPieceAt(board, 0, 7);
                string originalPosition = GetPieceAt(board, 0, 0);
                
                if (rookPosition == "WR" && originalPosition == ".")
                {
                    Console.WriteLine("    ✓ Promoted piece can be used immediately");
                }
                else
                {
                    Console.WriteLine($"    ✗ Promoted piece usage failed. New pos: {rookPosition}, Old pos: {originalPosition}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestCheckDetectionWithPromotedPieces()
        {
            Console.WriteLine("  Testing check detection works with promoted pieces...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotedPieceCheckScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("Q\n"); // Promote to Queen
            Console.SetIn(input);
            
            try
            {
                // Execute promotion that should put opponent in check
                board.MovePiece(1, 4, 0, 4);
                
                // Verify the promoted Queen puts Black king in check
                bool blackInCheck = board.IsKingInCheck("B");
                string promotedPiece = GetPieceAt(board, 0, 4);
                
                if (blackInCheck && promotedPiece == "WQ")
                {
                    Console.WriteLine("    ✓ Check detection works with promoted pieces");
                }
                else
                {
                    Console.WriteLine($"    ✗ Check detection failed. In check: {blackInCheck}, Piece: {promotedPiece}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestMultiplePromotionsInGame()
        {
            Console.WriteLine("Testing multiple promotions in same game...");
            
            ChessBoard board = new ChessBoard();
            SetupMultiplePromotionsScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("Q\nR\n"); // First Queen, then Rook
            Console.SetIn(input);
            
            try
            {
                // First promotion
                board.MovePiece(1, 0, 0, 0);
                string firstPromotion = GetPieceAt(board, 0, 0);
                
                // Skip a turn
                board.MovePiece(6, 7, 5, 7);
                
                // Second promotion
                board.MovePiece(1, 7, 0, 7);
                string secondPromotion = GetPieceAt(board, 0, 7);
                
                if (firstPromotion == "WQ" && secondPromotion == "WR")
                {
                    Console.WriteLine("  ✓ Multiple promotions in same game work correctly");
                }
                else
                {
                    Console.WriteLine($"  ✗ Multiple promotions failed. First: {firstPromotion}, Second: {secondPromotion}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestPromotionWithCapture()
        {
            Console.WriteLine("Testing promotion with capture...");
            
            ChessBoard board = new ChessBoard();
            SetupPromotionCaptureScenario(board);
            
            var originalIn = Console.In;
            var input = new StringReader("B\n"); // Promote to Bishop
            Console.SetIn(input);
            
            try
            {
                // Verify target square has enemy piece
                string targetPiece = GetPieceAt(board, 0, 1);
                
                // Execute promotion with capture
                board.MovePiece(1, 0, 0, 1);
                
                // Verify capture and promotion occurred
                string promotedPiece = GetPieceAt(board, 0, 1);
                string originalSquare = GetPieceAt(board, 1, 0);
                
                if (targetPiece == "BR" && promotedPiece == "WB" && originalSquare == ".")
                {
                    Console.WriteLine("  ✓ Promotion with capture works correctly");
                }
                else
                {
                    Console.WriteLine($"  ✗ Promotion capture failed. Target: {targetPiece}, Result: {promotedPiece}, Original: {originalSquare}");
                }
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        private static void TestAIPromotionInComplexPosition()
        {
            Console.WriteLine("Testing AI promotion in complex position...");
            
            ChessBoard board = new ChessBoard();
            SetupComplexAIPromotionScenario(board);
            
            // Verify initial complex position
            bool initialCheck = board.IsKingInCheck("W");
            
            // Execute AI promotion in complex scenario
            board.MovePieceAI(6, 3, 7, 3);
            
            // Verify AI handled complex promotion correctly
            string promotedPiece = GetPieceAt(board, 7, 3);
            bool finalCheck = board.IsKingInCheck("W");
            string currentTurn = board.CurrentTurn();
            
            if (promotedPiece == "BQ" && currentTurn == "W")
            {
                Console.WriteLine("  ✓ AI promotion in complex position works correctly");
            }
            else
            {
                Console.WriteLine($"  ✗ AI complex promotion failed. Piece: {promotedPiece}, Turn: {currentTurn}");
            }
        }

        // Helper methods for setting up test scenarios
        private static void SetupEndgamePromotionScenario(ChessBoard board, string color)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Set up basic endgame with promotion opportunity
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            if (color == "W")
            {
                boardArray[1, 3] = "WP"; // White pawn ready to promote
                SetCurrentTurn(board, "W");
            }
            else
            {
                boardArray[6, 3] = "BP"; // Black pawn ready to promote
                SetCurrentTurn(board, "B");
            }
        }

        private static void SetupPromotionEscapesCheckScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // White king in check, promotion can block/escape
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 4] = "WP"; // Pawn that can promote to block
            boardArray[0, 0] = "BR"; // Rook giving check
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupPromotionBlocksCheckScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup where promotion blocks check
            boardArray[7, 1] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 2] = "WP"; // Pawn can promote to block
            boardArray[0, 1] = "BQ"; // Queen giving check
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupPromotionCheckmateScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup where promotion delivers checkmate
            boardArray[7, 0] = "WK";
            boardArray[0, 7] = "BK"; // Black king cornered
            boardArray[1, 7] = "WP"; // Pawn promotes to deliver mate
            boardArray[0, 6] = "WR"; // Supporting piece
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupAIPromotionCheckmateScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup where AI promotion delivers checkmate
            boardArray[0, 0] = "BK";
            boardArray[7, 7] = "WK"; // White king cornered
            boardArray[6, 7] = "BP"; // Black pawn promotes to deliver mate
            boardArray[7, 6] = "BR"; // Supporting piece
            
            SetCurrentTurn(board, "B");
        }

        private static void SetupPostPromotionScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup for testing game flow after promotion
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 0] = "WP"; // Pawn to promote
            boardArray[1, 1] = "BP"; // Black pawn for subsequent move
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupPromotedPieceUsageScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup for testing promoted piece usage
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 0] = "WP"; // Pawn to promote to Rook
            boardArray[6, 1] = "BP"; // Black pawn for dummy move
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupPromotedPieceCheckScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup where promoted piece gives check
            boardArray[7, 0] = "WK";
            boardArray[0, 0] = "BK"; // Black king on same file as promotion
            boardArray[1, 4] = "WP"; // Pawn promotes to Queen giving check
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupMultiplePromotionsScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup for multiple promotions
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 0] = "WP"; // First pawn
            boardArray[1, 7] = "WP"; // Second pawn
            boardArray[6, 7] = "BP"; // Black pawn for dummy move
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupPromotionCaptureScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Setup promotion with capture
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[1, 0] = "WP"; // Pawn to promote
            boardArray[0, 1] = "BR"; // Black rook to capture
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupComplexAIPromotionScenario(ChessBoard board)
        {
            var boardArray = GetBoardArray(board);
            ClearBoard(boardArray);
            
            // Complex position with multiple pieces
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[6, 3] = "BP"; // Black pawn to promote
            boardArray[5, 2] = "WN"; // White knight
            boardArray[3, 5] = "BR"; // Black rook
            boardArray[2, 1] = "WB"; // White bishop
            
            SetCurrentTurn(board, "B");
        }

        // Utility helper methods
        private static string[,] GetBoardArray(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (string[,])boardField.GetValue(board);
        }

        private static void ClearBoard(string[,] boardArray)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
        }

        private static void SetCurrentTurn(ChessBoard board, string turn)
        {
            var turnField = typeof(ChessBoard).GetField("currentTurn", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            turnField.SetValue(board, turn);
        }

        private static string GetPieceAt(ChessBoard board, int row, int col)
        {
            var boardArray = GetBoardArray(board);
            return boardArray[row, col];
        }
    }
}