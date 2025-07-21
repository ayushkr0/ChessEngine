using System;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantTimingManagement
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant Timing Management ===");
            
            TestEnPassantOpportunityExpiration();
            TestEnPassantTimingValidation();
            TestEnPassantStateConsistency();
            TestMultipleTurnEnPassantTiming();
            TestEnPassantTimingWithDifferentMoves();
            TestEnPassantTimingEdgeCases();
            
            Console.WriteLine("=== En Passant Timing Management Tests Completed ===");
        }

        private static void TestEnPassantOpportunityExpiration()
        {
            Console.WriteLine("Testing en passant opportunity expiration...");
            
            ChessBoard board = new ChessBoard();
            SetupTwoSquarePawnMoveScenario(board);
            
            // Execute two-square pawn move to create opportunity
            board.MovePiece(6, 3, 4, 3); // White pawn e2-e4
            
            // Verify opportunity was created
            bool hasOpportunityAfterCreation = HasEnPassantOpportunity(board);
            string stateInfo = GetEnPassantStateInfo(board);
            
            if (hasOpportunityAfterCreation)
            {
                Console.WriteLine("✓ En passant opportunity created correctly");
                Console.WriteLine($"  State: {stateInfo}");
            }
            else
            {
                Console.WriteLine("✗ En passant opportunity not created");
            }
            
            // Make another move (not en passant) - should clear opportunity
            board.MovePiece(1, 0, 2, 0); // Black pawn a7-a6
            
            // Verify opportunity was cleared
            bool hasOpportunityAfterOtherMove = HasEnPassantOpportunity(board);
            
            if (!hasOpportunityAfterOtherMove)
            {
                Console.WriteLine("✓ En passant opportunity correctly expired after other move");
            }
            else
            {
                Console.WriteLine("✗ En passant opportunity should expire after other move");
            }
        }    
    private static void TestEnPassantTimingValidation()
        {
            Console.WriteLine("Testing en passant timing validation...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Valid timing scenario
            SetupValidEnPassantTimingScenario(board);
            bool validTiming = ValidateEnPassantTiming(board);
            
            if (validTiming)
            {
                Console.WriteLine("✓ Valid en passant timing correctly validated");
            }
            else
            {
                Console.WriteLine("✗ Valid en passant timing validation failed");
            }
            
            // Test 2: Invalid timing - victim pawn on wrong rank
            SetupInvalidEnPassantTimingScenario(board);
            bool invalidTiming = ValidateEnPassantTiming(board);
            
            if (!invalidTiming)
            {
                Console.WriteLine("✓ Invalid en passant timing correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid en passant timing incorrectly accepted");
            }
            
            // Test 3: No opportunity - should be valid
            ClearEnPassantOpportunity(board);
            bool noOpportunityTiming = ValidateEnPassantTiming(board);
            
            if (noOpportunityTiming)
            {
                Console.WriteLine("✓ No opportunity timing correctly validated");
            }
            else
            {
                Console.WriteLine("✗ No opportunity timing validation failed");
            }
        }

        private static void TestEnPassantStateConsistency()
        {
            Console.WriteLine("Testing en passant state consistency...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Consistent state after two-square move
            SetupTwoSquarePawnMoveScenario(board);
            board.MovePiece(6, 3, 4, 3); // White pawn two-square move
            
            bool consistentAfterMove = ValidateEnPassantConsistency(board);
            
            if (consistentAfterMove)
            {
                Console.WriteLine("✓ En passant state consistent after two-square move");
            }
            else
            {
                Console.WriteLine("✗ En passant state inconsistent after two-square move");
            }
            
            // Test 2: Consistent state after opportunity clearing
            board.MovePiece(1, 0, 2, 0); // Black move to clear opportunity
            
            bool consistentAfterClearing = ValidateEnPassantConsistency(board);
            
            if (consistentAfterClearing)
            {
                Console.WriteLine("✓ En passant state consistent after opportunity clearing");
            }
            else
            {
                Console.WriteLine("✗ En passant state inconsistent after opportunity clearing");
            }
            
            // Test 3: Consistency with manually corrupted state
            SetupCorruptedEnPassantState(board);
            bool consistentWithCorruption = ValidateEnPassantConsistency(board);
            
            if (!consistentWithCorruption)
            {
                Console.WriteLine("✓ Corrupted en passant state correctly detected as inconsistent");
            }
            else
            {
                Console.WriteLine("✗ Corrupted en passant state incorrectly validated as consistent");
            }
        }

        private static void TestMultipleTurnEnPassantTiming()
        {
            Console.WriteLine("Testing en passant timing across multiple turns...");
            
            ChessBoard board = new ChessBoard();
            SetupMultiTurnEnPassantScenario(board);
            
            // Turn 1: White makes two-square pawn move
            board.MovePiece(6, 4, 4, 4); // e2-e4
            bool hasOpportunityTurn1 = HasEnPassantOpportunity(board);
            
            // Turn 2: Black makes adjacent pawn move (could capture en passant)
            board.MovePiece(1, 3, 3, 3); // d7-d5
            bool hasOpportunityTurn2 = HasEnPassantOpportunity(board);
            
            // Turn 3: White makes different move (should clear Black's opportunity)
            board.MovePiece(6, 2, 4, 2); // c2-c4
            bool hasOpportunityTurn3 = HasEnPassantOpportunity(board);
            
            // Turn 4: Black could now capture White's c-pawn en passant
            bool canCaptureEnPassant = IsValidEnPassantMove(board, 3, 3, 4, 2, "B");
            
            if (hasOpportunityTurn1 && !hasOpportunityTurn2 && hasOpportunityTurn3 && canCaptureEnPassant)
            {
                Console.WriteLine("✓ Multi-turn en passant timing works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Multi-turn timing failed. T1: {hasOpportunityTurn1}, T2: {hasOpportunityTurn2}, T3: {hasOpportunityTurn3}, Can capture: {canCaptureEnPassant}");
            }
        } 
       private static void TestEnPassantTimingWithDifferentMoves()
        {
            Console.WriteLine("Testing en passant timing with different move types...");
            
            ChessBoard board = new ChessBoard();
            
            // Test different move types clearing en passant opportunities
            string[] moveTypes = { "Pawn", "Rook", "Knight", "Bishop", "Queen", "King" };
            bool[] results = new bool[moveTypes.Length];
            
            for (int i = 0; i < moveTypes.Length; i++)
            {
                SetupEnPassantWithDifferentPieces(board, moveTypes[i]);
                
                // Create en passant opportunity
                board.MovePiece(6, 3, 4, 3); // White pawn two-square move
                bool hasOpportunityBefore = HasEnPassantOpportunity(board);
                
                // Make move with different piece type
                ExecuteMoveByType(board, moveTypes[i]);
                bool hasOpportunityAfter = HasEnPassantOpportunity(board);
                
                results[i] = hasOpportunityBefore && !hasOpportunityAfter;
            }
            
            bool allMovesCleared = true;
            for (int i = 0; i < results.Length; i++)
            {
                if (!results[i])
                {
                    allMovesCleared = false;
                    Console.WriteLine($"✗ {moveTypes[i]} move failed to clear en passant opportunity");
                }
            }
            
            if (allMovesCleared)
            {
                Console.WriteLine("✓ All move types correctly clear en passant opportunities");
            }
        }

        private static void TestEnPassantTimingEdgeCases()
        {
            Console.WriteLine("Testing en passant timing edge cases...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: En passant opportunity with king in check
            SetupEnPassantWithCheckScenario(board);
            bool validWithCheck = ValidateEnPassantConsistency(board);
            
            if (validWithCheck)
            {
                Console.WriteLine("✓ En passant timing valid even with king in check");
            }
            else
            {
                Console.WriteLine("✗ En passant timing invalid with king in check");
            }
            
            // Test 2: Multiple consecutive two-square moves
            SetupConsecutiveTwoSquareMovesScenario(board);
            
            // First two-square move
            board.MovePiece(6, 3, 4, 3); // e2-e4
            string stateAfterFirst = GetEnPassantStateInfo(board);
            
            // Black move
            board.MovePiece(1, 0, 2, 0); // a7-a6
            
            // Second two-square move
            board.MovePiece(6, 5, 4, 5); // f2-f4
            string stateAfterSecond = GetEnPassantStateInfo(board);
            
            bool correctStateTransition = stateAfterFirst.Contains("(4,3)") && stateAfterSecond.Contains("(4,5)");
            
            if (correctStateTransition)
            {
                Console.WriteLine("✓ Consecutive two-square moves handle state transitions correctly");
            }
            else
            {
                Console.WriteLine($"✗ State transition failed. First: {stateAfterFirst}, Second: {stateAfterSecond}");
            }
            
            // Test 3: En passant opportunity expiration timing
            SetupEnPassantExpirationTest(board);
            
            // Create opportunity
            board.MovePiece(6, 2, 4, 2); // c2-c4
            bool hasOpportunityInitial = HasEnPassantOpportunity(board);
            
            // One move later - should still be available for opponent
            string currentTurn = board.CurrentTurn();
            bool shouldHaveOpportunity = (currentTurn == "B"); // Black's turn, so White's opportunity should be available
            
            // Make Black move (not en passant)
            board.MovePiece(1, 1, 2, 1); // b7-b6
            
            // Now it should be cleared
            bool hasOpportunityAfterBlackMove = HasEnPassantOpportunity(board);
            
            if (hasOpportunityInitial && shouldHaveOpportunity && !hasOpportunityAfterBlackMove)
            {
                Console.WriteLine("✓ En passant opportunity expiration timing works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Expiration timing failed. Initial: {hasOpportunityInitial}, Should have: {shouldHaveOpportunity}, After move: {hasOpportunityAfterBlackMove}");
            }
        }

        // Helper methods using reflection to access private methods
        private static bool HasEnPassantOpportunity(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("HasEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, null);
        }

        private static bool ValidateEnPassantTiming(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ValidateEnPassantTiming", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, null);
        }

        private static bool ValidateEnPassantConsistency(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ValidateEnPassantConsistency", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, null);
        }

        private static string GetEnPassantStateInfo(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("GetEnPassantStateInfo", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method.Invoke(board, null);
        }

        private static void ClearEnPassantOpportunity(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ClearEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, null);
        }

        private static bool IsValidEnPassantMove(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsValidEnPassantMove", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }        
// Helper methods for setting up test scenarios
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

        private static void SetEnPassantOpportunity(ChessBoard board, int pawnRow, int pawnCol)
        {
            var method = typeof(ChessBoard).GetMethod("SetEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { pawnRow, pawnCol });
        }

        private static void SetupTwoSquarePawnMoveScenario(ChessBoard board)
        {
            // Set up standard starting position
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
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupValidEnPassantTimingScenario(ChessBoard board)
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
            
            // Set up valid en passant scenario - White pawn on rank 4
            boardArray[4, 3] = "WP"; // White pawn that just moved two squares
            SetEnPassantOpportunity(board, 4, 3);
            SetCurrentTurn(board, "B");
        }

        private static void SetupInvalidEnPassantTimingScenario(ChessBoard board)
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
            
            // Set up invalid scenario - White pawn on wrong rank for en passant
            boardArray[5, 3] = "WP"; // White pawn on rank 5 (invalid for en passant)
            SetEnPassantOpportunity(board, 5, 3); // This creates invalid state
            SetCurrentTurn(board, "B");
        }

        private static void SetupCorruptedEnPassantState(ChessBoard board)
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
            
            // Create corrupted state - en passant opportunity but no victim pawn
            SetEnPassantOpportunity(board, 4, 3);
            // Don't place victim pawn - this creates inconsistent state
            SetCurrentTurn(board, "B");
        }

        private static void SetupMultiTurnEnPassantScenario(ChessBoard board)
        {
            SetupTwoSquarePawnMoveScenario(board); // Start with standard position
        }

        private static void SetupEnPassantWithDifferentPieces(ChessBoard board, string pieceType)
        {
            SetupTwoSquarePawnMoveScenario(board);
            
            // Modify board to have different piece types available for moves
            switch (pieceType)
            {
                case "Rook":
                    // Rook already in position
                    break;
                case "Knight":
                    // Knight already in position
                    break;
                case "Bishop":
                    // Move pawn to allow bishop movement
                    SetBoardPiece(board, 6, 2, ".");
                    break;
                case "Queen":
                    // Move pawn to allow queen movement
                    SetBoardPiece(board, 6, 3, ".");
                    break;
                case "King":
                    // King can move (though not recommended in opening)
                    break;
                default: // Pawn
                    // Pawn moves already available
                    break;
            }
        }

        private static void ExecuteMoveByType(ChessBoard board, string pieceType)
        {
            switch (pieceType)
            {
                case "Pawn":
                    board.MovePiece(1, 0, 2, 0); // a7-a6
                    break;
                case "Rook":
                    board.MovePiece(0, 0, 0, 1); // Ra8-b8
                    break;
                case "Knight":
                    board.MovePiece(0, 1, 2, 2); // Nb8-c6
                    break;
                case "Bishop":
                    board.MovePiece(0, 2, 1, 3); // Bc8-d7
                    break;
                case "Queen":
                    board.MovePiece(0, 3, 1, 3); // Qd8-d7
                    break;
                case "King":
                    board.MovePiece(0, 4, 0, 3); // Ke8-d8
                    break;
            }
        }

        private static void SetupEnPassantWithCheckScenario(ChessBoard board)
        {
            // Clear board
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up scenario with check
            boardArray[7, 4] = "WK";  // White king
            boardArray[0, 4] = "BK";  // Black king
            boardArray[4, 3] = "WP";  // White pawn (en passant victim)
            boardArray[4, 4] = "BP";  // Black pawn (can capture en passant)
            
            SetEnPassantOpportunity(board, 4, 3);
            SetCurrentTurn(board, "B");
        }

        private static void SetupConsecutiveTwoSquareMovesScenario(ChessBoard board)
        {
            SetupTwoSquarePawnMoveScenario(board);
        }

        private static void SetupEnPassantExpirationTest(ChessBoard board)
        {
            SetupTwoSquarePawnMoveScenario(board);
        }
    }
}