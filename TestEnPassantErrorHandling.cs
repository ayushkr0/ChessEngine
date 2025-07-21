using System;
using System.IO;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantErrorHandling
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant Error Handling and User Feedback ===");
            
            TestEnPassantErrorMessages();
            TestEnPassantUserFeedback();
            TestEnPassantTimingErrors();
            TestEnPassantConsistencyErrors();
            TestEnPassantStateInfo();
            TestEnPassantEdgeCaseErrors();
            
            Console.WriteLine("=== En Passant Error Handling Tests Completed ===");
        }

        private static void TestEnPassantErrorMessages()
        {
            Console.WriteLine("Testing en passant error messages...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: No opportunity error
            string noOpportunityError = GetEnPassantCaptureError(board, 3, 4, 2, 5, "W");
            if (noOpportunityError.Contains("No en passant opportunity"))
            {
                Console.WriteLine("✓ No opportunity error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected 'No opportunity' message, got: {noOpportunityError}");
            }
            
            // Test 2: Wrong piece error
            SetupEnPassantWithWrongPiece(board);
            string wrongPieceError = GetEnPassantCaptureError(board, 3, 4, 2, 5, "W");
            if (wrongPieceError.Contains("Only pawns can execute"))
            {
                Console.WriteLine("✓ Wrong piece error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected 'Only pawns' message, got: {wrongPieceError}");
            }
            
            // Test 3: Wrong direction error
            SetupEnPassantWithWrongDirection(board);
            string wrongDirectionError = GetEnPassantCaptureError(board, 3, 4, 4, 5, "W");
            if (wrongDirectionError.Contains("must move"))
            {
                Console.WriteLine("✓ Wrong direction error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected direction message, got: {wrongDirectionError}");
            }
            
            // Test 4: Wrong rank error
            SetupEnPassantWithWrongRank(board);
            string wrongRankError = GetIsValidEnPassantMoveError(board, 2, 4, 1, 5, "W");
            if (wrongRankError.Contains("must be on rank"))
            {
                Console.WriteLine("✓ Wrong rank error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected rank message, got: {wrongRankError}");
            }
        }

        private static void TestEnPassantUserFeedback()
        {
            Console.WriteLine("Testing en passant user feedback...");
            
            ChessBoard board = new ChessBoard();
            
            // Capture console output to verify feedback messages
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            try
            {
                // Test 1: Human en passant capture feedback
                SetupValidEnPassantScenario(board, "W");
                board.MovePiece(3, 4, 2, 5); // Execute en passant
                
                string humanOutput = stringWriter.ToString();
                bool hasAlgebraicNotation = humanOutput.Contains("pawn") && 
                                          (humanOutput.Contains("e5-d6") || 
                                           humanOutput.Contains("e5") && humanOutput.Contains("d6"));
                
                Console.SetOut(originalOut);
                if (hasAlgebraicNotation)
                {
                    Console.WriteLine("✓ Human en passant feedback includes algebraic notation");
                }
                else
                {
                    Console.WriteLine($"✗ Human feedback missing algebraic notation: {humanOutput}");
                }
                
                // Reset output capture
                stringWriter = new StringWriter();
                Console.SetOut(stringWriter);
                
                // Test 2: AI en passant capture feedback
                SetupValidEnPassantScenario(board, "W");
                board.MovePieceAI(3, 4, 2, 5);
                
                string aiOutput = stringWriter.ToString();
                bool hasAIIndicator = aiOutput.Contains("AI") && aiOutput.Contains("executes en passant");
                
                Console.SetOut(originalOut);
                if (hasAIIndicator)
                {
                    Console.WriteLine("✓ AI en passant feedback clearly indicates AI move");
                }
                else
                {
                    Console.WriteLine($"✗ AI feedback missing AI indicator: {aiOutput}");
                }
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        private static void TestEnPassantTimingErrors()
        {
            Console.WriteLine("Testing en passant timing error messages...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Victim on wrong rank
            SetupEnPassantWithVictimOnWrongRank(board);
            string wrongRankError = GetEnPassantTimingError(board);
            if (wrongRankError.Contains("must be on rank"))
            {
                Console.WriteLine("✓ Victim wrong rank error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected rank message, got: {wrongRankError}");
            }
            
            // Test 2: Missing victim pawn
            SetupEnPassantWithMissingVictim(board);
            string missingVictimError = GetEnPassantTimingError(board);
            if (missingVictimError.Contains("victim pawn is missing"))
            {
                Console.WriteLine("✓ Missing victim error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected missing victim message, got: {missingVictimError}");
            }
            
            // Test 3: Victim not a pawn
            SetupEnPassantWithNonPawnVictim(board);
            string nonPawnError = GetEnPassantTimingError(board);
            if (nonPawnError.Contains("must be a pawn"))
            {
                Console.WriteLine("✓ Non-pawn victim error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected non-pawn message, got: {nonPawnError}");
            }
        }

        private static void TestEnPassantConsistencyErrors()
        {
            Console.WriteLine("Testing en passant consistency error messages...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Target square not empty
            SetupEnPassantWithOccupiedTarget(board);
            string occupiedTargetError = GetEnPassantConsistencyError(board);
            if (occupiedTargetError.Contains("must be empty"))
            {
                Console.WriteLine("✓ Occupied target error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected occupied target message, got: {occupiedTargetError}");
            }
            
            // Test 2: Invalid geometric relationship
            SetupEnPassantWithInvalidGeometry(board);
            string geometryError = GetEnPassantConsistencyError(board);
            if (geometryError.Contains("geometric relationship"))
            {
                Console.WriteLine("✓ Invalid geometry error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected geometry message, got: {geometryError}");
            }
            
            // Test 3: Out of bounds target
            SetupEnPassantWithOutOfBoundsTarget(board);
            string outOfBoundsError = GetEnPassantConsistencyError(board);
            if (outOfBoundsError.Contains("out of bounds"))
            {
                Console.WriteLine("✓ Out of bounds error message is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected out of bounds message, got: {outOfBoundsError}");
            }
        }

        private static void TestEnPassantStateInfo()
        {
            Console.WriteLine("Testing enhanced en passant state info...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: No opportunity
            string noOpportunityInfo = GetEnPassantStateInfo(board);
            if (noOpportunityInfo.Contains("No en passant opportunity"))
            {
                Console.WriteLine("✓ No opportunity state info is clear");
            }
            else
            {
                Console.WriteLine($"✗ Expected no opportunity message, got: {noOpportunityInfo}");
            }
            
            // Test 2: Valid opportunity
            SetupValidEnPassantScenario(board, "W");
            string validInfo = GetEnPassantStateInfo(board);
            bool hasDetailedInfo = validInfo.Contains("Target square") && 
                                 validInfo.Contains("Victim pawn") && 
                                 validInfo.Contains("Status: Valid");
            
            if (hasDetailedInfo)
            {
                Console.WriteLine("✓ Valid opportunity state info is detailed");
            }
            else
            {
                Console.WriteLine($"✗ Expected detailed info, got: {validInfo}");
            }
            
            // Test 3: Invalid opportunity
            SetupInvalidEnPassantState(board);
            string invalidInfo = GetEnPassantStateInfo(board);
            if (invalidInfo.Contains("Status: Invalid"))
            {
                Console.WriteLine("✓ Invalid opportunity state info shows error");
            }
            else
            {
                Console.WriteLine($"✗ Expected invalid status, got: {invalidInfo}");
            }
        }

        private static void TestEnPassantEdgeCaseErrors()
        {
            Console.WriteLine("Testing en passant edge case error handling...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Can execute validation with detailed error
            SetupInvalidEnPassantState(board);
            string canExecuteError = GetCanExecuteEnPassantCaptureError(board, 3, 4, 2, 5, "W");
            if (!string.IsNullOrEmpty(canExecuteError))
            {
                Console.WriteLine($"✓ Can execute validation provides error: {canExecuteError}");
            }
            else
            {
                Console.WriteLine("✗ Can execute validation missing error message");
            }
            
            // Test 2: Invalid move attempt feedback
            var originalOut = Console.Out;
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            try
            {
                // Setup scenario where it looks like en passant but isn't valid
                SetupInvalidEnPassantAttempt(board);
                board.MovePiece(3, 4, 2, 5); // This should show an error message
                
                string output = stringWriter.ToString();
                bool hasErrorFeedback = output.Contains("Invalid en passant");
                
                Console.SetOut(originalOut);
                if (hasErrorFeedback)
                {
                    Console.WriteLine("✓ Invalid attempt provides user feedback");
                }
                else
                {
                    Console.WriteLine($"✗ Expected error feedback, got: {output}");
                }
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        // Helper methods using reflection to access private methods
        private static string GetEnPassantCaptureError(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string).MakeByRefType() }, 
                null);
            
            if (method == null) return "Method not found";
            
            var parameters = new object[] { fromRow, fromCol, toRow, toCol, color, null };
            method.Invoke(board, parameters);
            return parameters[5]?.ToString() ?? "No error";
        }

        private static string GetIsValidEnPassantMoveError(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsValidEnPassantMove", 
                BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string).MakeByRefType() }, 
                null);
            
            if (method == null) return "Method not found";
            
            var parameters = new object[] { fromRow, fromCol, toRow, toCol, color, null };
            method.Invoke(board, parameters);
            return parameters[5]?.ToString() ?? "No error";
        }

        private static string GetEnPassantTimingError(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ValidateEnPassantTiming", 
                BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new Type[] { typeof(string).MakeByRefType() }, 
                null);
            
            if (method == null) return "Method not found";
            
            var parameters = new object[] { null };
            method.Invoke(board, parameters);
            return parameters[0]?.ToString() ?? "No error";
        }

        private static string GetEnPassantConsistencyError(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ValidateEnPassantConsistency", 
                BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new Type[] { typeof(string).MakeByRefType() }, 
                null);
            
            if (method == null) return "Method not found";
            
            var parameters = new object[] { null };
            method.Invoke(board, parameters);
            return parameters[0]?.ToString() ?? "No error";
        }

        private static string GetCanExecuteEnPassantCaptureError(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("CanExecuteEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance, null, 
                new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string).MakeByRefType() }, 
                null);
            
            if (method == null) return "Method not found";
            
            var parameters = new object[] { fromRow, fromCol, toRow, toCol, color, null };
            method.Invoke(board, parameters);
            return parameters[5]?.ToString() ?? "No error";
        }

        private static string GetEnPassantStateInfo(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("GetEnPassantStateInfo", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method.Invoke(board, null);
        }

        private static void SetEnPassantOpportunity(ChessBoard board, int pawnRow, int pawnCol)
        {
            var method = typeof(ChessBoard).GetMethod("SetEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { pawnRow, pawnCol });
        }

        private static void SetEnPassantTarget(ChessBoard board, int row, int col)
        {
            var field = typeof(ChessBoard).GetField("enPassantTarget", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(board, (row, col));
        }

        private static void SetEnPassantVictim(ChessBoard board, int row, int col)
        {
            var field = typeof(ChessBoard).GetField("enPassantVictim", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(board, (row, col));
        }

        private static void SetBoardPiece(ChessBoard board, int row, int col, string piece)
        {
            var field = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var boardArray = (string[,])field.GetValue(board);
            boardArray[row, col] = piece;
        }

        // Test scenario setup methods
        private static void SetupEnPassantWithWrongPiece(ChessBoard board)
        {
            // Setup a scenario where a non-pawn tries to do en passant
            SetBoardPiece(board, 3, 4, "WR"); // Rook instead of pawn
            SetBoardPiece(board, 3, 5, "BP"); // Black pawn victim
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithWrongDirection(ChessBoard board)
        {
            // Setup a scenario where pawn tries to move in wrong direction
            SetBoardPiece(board, 3, 4, "WP"); // White pawn
            SetBoardPiece(board, 3, 5, "BP"); // Black pawn victim
            SetEnPassantTarget(board, 4, 5); // Wrong direction target
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithWrongRank(ChessBoard board)
        {
            // Setup a scenario where pawn is on wrong rank
            SetBoardPiece(board, 2, 4, "WP"); // White pawn on wrong rank
            SetBoardPiece(board, 3, 5, "BP"); // Black pawn victim
            SetEnPassantTarget(board, 1, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupValidEnPassantScenario(ChessBoard board, string color)
        {
            // Setup a valid en passant scenario
            if (color == "W")
            {
                SetBoardPiece(board, 3, 4, "WP"); // White pawn on 5th rank
                SetBoardPiece(board, 3, 5, "BP"); // Black pawn victim on 5th rank
                SetEnPassantTarget(board, 2, 5); // Target on 6th rank
                SetEnPassantVictim(board, 3, 5); // Victim on 5th rank
            }
            else
            {
                SetBoardPiece(board, 4, 4, "BP"); // Black pawn on 4th rank
                SetBoardPiece(board, 4, 5, "WP"); // White pawn victim on 4th rank
                SetEnPassantTarget(board, 5, 5); // Target on 3rd rank
                SetEnPassantVictim(board, 4, 5); // Victim on 4th rank
            }
        }

        private static void SetupEnPassantWithVictimOnWrongRank(ChessBoard board)
        {
            // Setup with victim pawn on wrong rank
            SetBoardPiece(board, 2, 5, "BP"); // Black pawn on wrong rank
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 2, 5); // Wrong rank for victim
        }

        private static void SetupEnPassantWithMissingVictim(ChessBoard board)
        {
            // Setup with missing victim pawn
            SetBoardPiece(board, 3, 5, "."); // Empty square where victim should be
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithNonPawnVictim(ChessBoard board)
        {
            // Setup with non-pawn as victim
            SetBoardPiece(board, 3, 5, "BR"); // Rook instead of pawn
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithOccupiedTarget(ChessBoard board)
        {
            // Setup with occupied target square
            SetBoardPiece(board, 3, 5, "BP"); // Victim pawn
            SetBoardPiece(board, 2, 5, "WR"); // Occupied target square
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithInvalidGeometry(ChessBoard board)
        {
            // Setup with invalid geometric relationship
            SetBoardPiece(board, 3, 5, "BP"); // Victim pawn
            SetEnPassantTarget(board, 1, 6); // Wrong geometric relationship
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupEnPassantWithOutOfBoundsTarget(ChessBoard board)
        {
            // Setup with out of bounds target
            SetBoardPiece(board, 3, 5, "BP"); // Victim pawn
            SetEnPassantTarget(board, -1, 5); // Out of bounds target
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupInvalidEnPassantState(ChessBoard board)
        {
            // Setup an invalid en passant state for testing
            SetBoardPiece(board, 3, 5, "BR"); // Wrong piece type as victim
            SetEnPassantTarget(board, 2, 5);
            SetEnPassantVictim(board, 3, 5);
        }

        private static void SetupInvalidEnPassantAttempt(ChessBoard board)
        {
            // Setup a scenario that looks like en passant but isn't valid
            SetBoardPiece(board, 3, 4, "WP"); // White pawn
            SetBoardPiece(board, 2, 5, "."); // Empty target square
            // No en passant opportunity set - this should trigger error feedback
        }
    }
}