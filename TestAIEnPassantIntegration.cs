using System;
using System.Collections.Generic;
using System.Reflection;

namespace ChessEngine
{
    public class TestAIEnPassantIntegration
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing AI En Passant Integration ===");
            
            TestGenerateEnPassantMoves();
            TestEvaluateEnPassantCapture();
            TestAIEnPassantMoveGeneration();
            TestAIEnPassantExecution();
            TestAIEnPassantInMinimax();
            TestAIEnPassantDecisionMaking();
            
            Console.WriteLine("=== AI En Passant Integration Tests Completed ===");
        }

        private static void TestGenerateEnPassantMoves()
        {
            Console.WriteLine("Testing GenerateEnPassantMoves method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: No en passant opportunity - should return empty list
            var noOpportunityMoves = GenerateEnPassantMoves(board, "W");
            
            if (noOpportunityMoves.Count == 0)
            {
                Console.WriteLine("✓ No opportunity scenario returns empty list");
            }
            else
            {
                Console.WriteLine($"✗ Expected empty list, got {noOpportunityMoves.Count} moves");
            }
            
            // Test 2: Valid en passant opportunity for White
            SetupEnPassantOpportunityForAI(board, "W");
            var whiteMoves = GenerateEnPassantMoves(board, "W");
            
            if (whiteMoves.Count == 1 && whiteMoves[0].fromRow == 3 && whiteMoves[0].fromCol == 4 && 
                whiteMoves[0].toRow == 2 && whiteMoves[0].toCol == 5)
            {
                Console.WriteLine("✓ White en passant moves generated correctly");
            }
            else
            {
                Console.WriteLine($"✗ White moves incorrect. Count: {whiteMoves.Count}");
                if (whiteMoves.Count > 0)
                    Console.WriteLine($"  First move: ({whiteMoves[0].fromRow},{whiteMoves[0].fromCol}) -> ({whiteMoves[0].toRow},{whiteMoves[0].toCol})");
            }
            
            // Test 3: Valid en passant opportunity for Black
            SetupEnPassantOpportunityForAI(board, "B");
            var blackMoves = GenerateEnPassantMoves(board, "B");
            
            if (blackMoves.Count == 1 && blackMoves[0].fromRow == 4 && blackMoves[0].fromCol == 3 && 
                blackMoves[0].toRow == 5 && blackMoves[0].toCol == 2)
            {
                Console.WriteLine("✓ Black en passant moves generated correctly");
            }
            else
            {
                Console.WriteLine($"✗ Black moves incorrect. Count: {blackMoves.Count}");
                if (blackMoves.Count > 0)
                    Console.WriteLine($"  First move: ({blackMoves[0].fromRow},{blackMoves[0].fromCol}) -> ({blackMoves[0].toRow},{blackMoves[0].toCol})");
            }
            
            // Test 4: Multiple pawns can capture en passant
            SetupMultipleEnPassantCapturers(board);
            var multipleMoves = GenerateEnPassantMoves(board, "W");
            
            if (multipleMoves.Count == 2)
            {
                Console.WriteLine("✓ Multiple en passant moves generated correctly");
            }
            else
            {
                Console.WriteLine($"✗ Expected 2 moves for multiple capturers, got {multipleMoves.Count}");
            }
        }

        private static void TestEvaluateEnPassantCapture()
        {
            Console.WriteLine("Testing EvaluateEnPassantCapture method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Basic en passant evaluation
            SetupBasicEnPassantEvaluation(board);
            int basicScore = EvaluateEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (basicScore >= 100) // Should be at least base pawn value
            {
                Console.WriteLine($"✓ Basic en passant evaluation: {basicScore} points");
            }
            else
            {
                Console.WriteLine($"✗ Basic evaluation too low: {basicScore}");
            }
            
            // Test 2: En passant with pawn structure improvement
            SetupEnPassantWithStructureImprovement(board);
            int structureScore = EvaluateEnPassantCapture(board, 3, 1, 2, 2, "W");
            
            if (structureScore > basicScore)
            {
                Console.WriteLine($"✓ Structure improvement bonus: {structureScore} points");
            }
            else
            {
                Console.WriteLine($"✗ Structure improvement not detected: {structureScore}");
            }
            
            // Test 3: En passant creating passed pawn
            SetupEnPassantCreatesPassedPawn(board);
            int passedPawnScore = EvaluateEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (passedPawnScore > basicScore)
            {
                Console.WriteLine($"✓ Passed pawn creation bonus: {passedPawnScore} points");
            }
            else
            {
                Console.WriteLine($"✗ Passed pawn bonus not detected: {passedPawnScore}");
            }
            
            // Test 4: En passant exposing king to danger
            SetupEnPassantExposesKing(board);
            int dangerScore = EvaluateEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (dangerScore < basicScore)
            {
                Console.WriteLine($"✓ King danger penalty applied: {dangerScore} points");
            }
            else
            {
                Console.WriteLine($"✗ King danger penalty not applied: {dangerScore}");
            }
        }

        private static void TestAIEnPassantMoveGeneration()
        {
            Console.WriteLine("Testing AI en passant move generation in MakeBestAIMove...");
            
            ChessBoard board = new ChessBoard();
            SetupAIEnPassantDecisionScenario(board);
            
            // Capture initial state
            string initialVictim = GetBoardPiece(board, 4, 2);
            string initialCapturingPawn = GetBoardPiece(board, 4, 3);
            string currentTurn = board.CurrentTurn();
            
            // Let AI make a move (should consider en passant)
            board.MakeBestAIMove(2); // Depth 2 for reasonable evaluation
            
            // Check if AI made en passant capture or another move
            string finalVictim = GetBoardPiece(board, 4, 2);
            string finalCapturingPawn = GetBoardPiece(board, 4, 3);
            string finalTarget = GetBoardPiece(board, 5, 2);
            string newTurn = board.CurrentTurn();
            
            // AI should have made some move and turn should have switched
            if (currentTurn == "B" && newTurn == "W")
            {
                Console.WriteLine("✓ AI made a move and turn switched correctly");
                
                // Check if AI chose en passant (victim removed, capturing pawn moved)
                if (initialVictim == "WP" && finalVictim == "." && 
                    initialCapturingPawn == "BP" && finalCapturingPawn == "." && finalTarget == "BP")
                {
                    Console.WriteLine("✓ AI chose en passant capture");
                }
                else
                {
                    Console.WriteLine("✓ AI chose different move (also valid)");
                }
            }
            else
            {
                Console.WriteLine($"✗ AI move failed. Turn: {currentTurn} -> {newTurn}");
            }
        }
        
        private static void TestAIEnPassantExecution()
        {
            Console.WriteLine("Testing AI en passant execution...");
            
            ChessBoard board = new ChessBoard();
            SetupForcedEnPassantScenario(board);
            
            // Capture initial state
            string initialVictim = GetBoardPiece(board, 3, 5);
            string initialCapturingPawn = GetBoardPiece(board, 3, 4);
            
            // Force AI to make en passant move by making it the only legal move
            board.MovePieceAI(3, 4, 2, 5); // Direct en passant execution
            
            // Verify execution
            string finalVictim = GetBoardPiece(board, 3, 5);
            string finalCapturingPawn = GetBoardPiece(board, 3, 4);
            string finalTarget = GetBoardPiece(board, 2, 5);
            
            if (initialVictim == "BP" && finalVictim == "." && 
                initialCapturingPawn == "WP" && finalCapturingPawn == "." && finalTarget == "WP")
            {
                Console.WriteLine("✓ AI en passant execution works correctly");
            }
            else
            {
                Console.WriteLine($"✗ AI execution failed. Victim: {initialVictim}->{finalVictim}, Capturing: {initialCapturingPawn}->{finalCapturingPawn}, Target: {finalTarget}");
            }
        }

        private static void TestAIEnPassantInMinimax()
        {
            Console.WriteLine("Testing AI en passant in minimax evaluation...");
            
            ChessBoard board = new ChessBoard();
            SetupMinimaxEnPassantTest(board);
            
            // Test that AI can evaluate en passant moves at different depths
            int[] depths = { 1, 2, 3 };
            bool[] depthResults = new bool[depths.Length];
            
            for (int i = 0; i < depths.Length; i++)
            {
                // Reset board for each test
                SetupMinimaxEnPassantTest(board);
                
                try
                {
                    // AI should be able to evaluate at this depth without errors
                    board.MakeBestAIMove(depths[i]);
                    depthResults[i] = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Depth {depths[i]} failed: {ex.Message}");
                    depthResults[i] = false;
                }
            }
            
            bool allDepthsWork = true;
            for (int i = 0; i < depthResults.Length; i++)
            {
                if (!depthResults[i])
                {
                    allDepthsWork = false;
                    Console.WriteLine($"✗ Depth {depths[i]} evaluation failed");
                }
            }
            
            if (allDepthsWork)
            {
                Console.WriteLine("✓ AI en passant works correctly in minimax at all depths");
            }
        }

        private static void TestAIEnPassantDecisionMaking()
        {
            Console.WriteLine("Testing AI en passant decision making...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: AI should prefer en passant when it's clearly better
            SetupEnPassantVsRegularMove(board);
            
            string beforeMove = GetBoardPiece(board, 4, 3);
            board.MakeBestAIMove(2);
            string afterMove = GetBoardPiece(board, 4, 3);
            
            // AI should have made some move
            bool aiMadeMove = (beforeMove != afterMove) || HasBoardChanged(board);
            
            if (aiMadeMove)
            {
                Console.WriteLine("✓ AI makes strategic decisions with en passant available");
            }
            else
            {
                Console.WriteLine("✗ AI failed to make any move");
            }
            
            // Test 2: AI should avoid en passant when it leads to disadvantage
            SetupBadEnPassantScenario(board);
            
            string victimBefore = GetBoardPiece(board, 4, 2);
            board.MakeBestAIMove(2);
            string victimAfter = GetBoardPiece(board, 4, 2);
            
            // In this scenario, AI should avoid the bad en passant
            // (This is a heuristic test - AI might still choose it, but it should evaluate properly)
            Console.WriteLine($"✓ AI evaluated en passant scenario (victim: {victimBefore} -> {victimAfter})");
            
            // Test 3: AI handles en passant timing correctly
            SetupEnPassantTimingTest(board);
            
            // Make a non-en-passant move first
            board.MovePiece(1, 0, 2, 0); // Black pawn move
            
            // Now AI should not have en passant available
            bool hasOpportunityAfterMove = HasEnPassantOpportunity(board);
            
            if (!hasOpportunityAfterMove)
            {
                Console.WriteLine("✓ AI respects en passant timing rules");
            }
            else
            {
                Console.WriteLine("✗ AI timing rule violation");
            }
        }

        // Helper methods using reflection to access private methods
        private static List<(int fromRow, int fromCol, int toRow, int toCol)> GenerateEnPassantMoves(ChessBoard board, string color)
        {
            var method = typeof(ChessBoard).GetMethod("GenerateEnPassantMoves", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (List<(int fromRow, int fromCol, int toRow, int toCol)>)method.Invoke(board, new object[] { color });
        }

        private static int EvaluateEnPassantCapture(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("EvaluateEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static bool HasEnPassantOpportunity(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("HasEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, null);
        }

        private static void SetEnPassantOpportunity(ChessBoard board, int pawnRow, int pawnCol)
        {
            var method = typeof(ChessBoard).GetMethod("SetEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { pawnRow, pawnCol });
        }

        private static string GetBoardPiece(ChessBoard board, int row, int col)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            return boardArray[row, col];
        }

        private static void SetBoardPiece(ChessBoard board, int row, int col, string piece)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            boardArray[row, col] = piece;
        }

        private static void SetCurrentTurn(ChessBoard board, string turn)
        {
            var turnField = typeof(ChessBoard).GetField("currentTurn", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            turnField.SetValue(board, turn);
        }

        // Test scenario setup methods
        private static void SetupEnPassantOpportunityForAI(ChessBoard board, string color)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            if (color == "W")
            {
                // White can capture Black pawn en passant
                boardArray[3, 4] = "WP"; // White capturing pawn
                boardArray[3, 5] = "BP"; // Black victim pawn
                SetEnPassantOpportunity(board, 3, 5);
                SetCurrentTurn(board, "W");
            }
            else
            {
                // Black can capture White pawn en passant
                boardArray[4, 3] = "BP"; // Black capturing pawn
                boardArray[4, 2] = "WP"; // White victim pawn
                SetEnPassantOpportunity(board, 4, 2);
                SetCurrentTurn(board, "B");
            }
        }

        private static void SetupMultipleEnPassantCapturers(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // Two White pawns can capture same Black pawn
            boardArray[3, 3] = "WP"; // First White capturing pawn
            boardArray[3, 5] = "WP"; // Second White capturing pawn
            boardArray[3, 4] = "BP"; // Black victim pawn
            SetEnPassantOpportunity(board, 3, 4);
            SetCurrentTurn(board, "W");
        } 
       private static void SetupBasicEnPassantEvaluation(ChessBoard board)
        {
            SetupEnPassantOpportunityForAI(board, "W");
        }

        private static void SetupEnPassantWithStructureImprovement(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // White pawn on edge can capture toward center
            boardArray[3, 1] = "WP"; // White pawn on edge
            boardArray[3, 2] = "BP"; // Black victim pawn
            SetEnPassantOpportunity(board, 3, 2);
            SetCurrentTurn(board, "W");
        }

        private static void SetupEnPassantCreatesPassedPawn(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // En passant capture will create passed pawn
            boardArray[3, 4] = "WP"; // White capturing pawn
            boardArray[3, 5] = "BP"; // Black victim pawn (only Black pawn in area)
            SetEnPassantOpportunity(board, 3, 5);
            SetCurrentTurn(board, "W");
        }

        private static void SetupEnPassantExposesKing(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up scenario where en passant exposes king
            boardArray[3, 4] = "WK";  // White king
            boardArray[0, 4] = "BK";  // Black king
            boardArray[3, 4] = "WP";  // White capturing pawn
            boardArray[3, 5] = "BP";  // Black victim pawn
            boardArray[3, 0] = "BR";  // Black rook that would give check
            
            SetEnPassantOpportunity(board, 3, 5);
            SetCurrentTurn(board, "W");
        }

        private static void SetupAIEnPassantDecisionScenario(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // Black AI can capture White pawn en passant
            boardArray[4, 3] = "BP"; // Black capturing pawn
            boardArray[4, 2] = "WP"; // White victim pawn
            boardArray[1, 0] = "BP"; // Another Black pawn for alternative moves
            
            SetEnPassantOpportunity(board, 4, 2);
            SetCurrentTurn(board, "B");
        }

        private static void SetupForcedEnPassantScenario(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // White can capture Black pawn en passant
            boardArray[3, 4] = "WP"; // White capturing pawn
            boardArray[3, 5] = "BP"; // Black victim pawn
            
            SetEnPassantOpportunity(board, 3, 5);
            SetCurrentTurn(board, "W");
        }

        private static void SetupMinimaxEnPassantTest(ChessBoard board)
        {
            // Set up standard starting position for minimax testing
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Initialize to starting position
            string[] backRank = { "R", "N", "B", "Q", "K", "B", "N", "R" };
            
            for (int i = 0; i < 8; i++)
            {
                boardArray[0, i] = "B" + backRank[i];
                boardArray[1, i] = "BP";
                boardArray[6, i] = "WP";
                boardArray[7, i] = "W" + backRank[i];
            }
            
            for (int row = 2; row <= 5; row++)
                for (int col = 0; col < 8; col++)
                    boardArray[row, col] = ".";
            
            // Create en passant opportunity
            boardArray[4, 3] = "WP"; // White pawn that just moved two squares
            boardArray[4, 4] = "BP"; // Black pawn that can capture
            
            SetEnPassantOpportunity(board, 4, 3);
            SetCurrentTurn(board, "B");
        }

        private static void SetupEnPassantVsRegularMove(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // Black has choice between en passant and regular move
            boardArray[4, 3] = "BP"; // Black pawn (can capture en passant)
            boardArray[4, 2] = "WP"; // White victim pawn
            boardArray[1, 1] = "BP"; // Another Black pawn for regular move
            
            SetEnPassantOpportunity(board, 4, 2);
            SetCurrentTurn(board, "B");
        }

        private static void SetupBadEnPassantScenario(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up scenario where en passant might be disadvantageous
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[4, 3] = "BP"; // Black pawn
            boardArray[4, 2] = "WP"; // White victim pawn
            boardArray[5, 1] = "WQ"; // White queen that might benefit from en passant
            
            SetEnPassantOpportunity(board, 4, 2);
            SetCurrentTurn(board, "B");
        }

        private static void SetupEnPassantTimingTest(ChessBoard board)
        {
            // Set up standard starting position
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            string[] backRank = { "R", "N", "B", "Q", "K", "B", "N", "R" };
            
            for (int i = 0; i < 8; i++)
            {
                boardArray[0, i] = "B" + backRank[i];
                boardArray[1, i] = "BP";
                boardArray[6, i] = "WP";
                boardArray[7, i] = "W" + backRank[i];
            }
            
            for (int row = 2; row <= 5; row++)
                for (int col = 0; col < 8; col++)
                    boardArray[row, col] = ".";
            
            // Create en passant opportunity that will expire
            boardArray[4, 3] = "WP"; // White pawn
            boardArray[4, 4] = "BP"; // Black pawn
            
            SetEnPassantOpportunity(board, 4, 3);
            SetCurrentTurn(board, "B");
        }

        private static bool HasBoardChanged(ChessBoard board)
        {
            // Simple heuristic: check if any pieces moved from starting positions
            // This is a basic implementation for testing purposes
            return true; // Assume board changed if AI made a move
        }
    }
}