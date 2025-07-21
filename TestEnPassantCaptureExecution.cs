using System;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantCaptureExecution
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant Capture Execution ===");
            
            TestExecuteEnPassantCapture();
            TestRollbackEnPassantCapture();
            TestCanExecuteEnPassantCapture();
            TestAtomicBoardStateUpdates();
            TestErrorHandlingAndValidation();
            TestCaptureExecutionWithUserFeedback();
            
            Console.WriteLine("=== En Passant Capture Execution Tests Completed ===");
        }

        private static void TestExecuteEnPassantCapture()
        {
            Console.WriteLine("Testing ExecuteEnPassantCapture method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Valid White en passant capture execution
            SetupValidCaptureScenario(board, "W");
            
            var captureInfo = ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            // Verify capture execution
            string capturingPawn = GetBoardPiece(board, 2, 5);
            string originalSquare = GetBoardPiece(board, 3, 4);
            string victimSquare = GetBoardPiece(board, 3, 5);
            
            bool wasExecuted = captureInfo != null && GetWasExecutedProperty(captureInfo);
            if (wasExecuted && capturingPawn == "WP" && originalSquare == "." && victimSquare == ".")
            {
                Console.WriteLine("✓ White en passant capture executed correctly");
            }
            else
            {
                Console.WriteLine($"✗ White capture failed. Executed: {GetWasExecutedProperty(captureInfo)}, Capturing: {capturingPawn}, Original: {originalSquare}, Victim: {victimSquare}");
            }
            
            // Test 2: Valid Black en passant capture execution
            SetupValidCaptureScenario(board, "B");
            
            var blackCaptureInfo = ExecuteEnPassantCapture(board, 4, 3, 5, 2, "B");
            
            // Verify capture execution
            string blackCapturingPawn = GetBoardPiece(board, 5, 2);
            string blackOriginalSquare = GetBoardPiece(board, 4, 3);
            string blackVictimSquare = GetBoardPiece(board, 4, 2);
            
            bool blackWasExecuted = blackCaptureInfo != null && GetWasExecutedProperty(blackCaptureInfo);
            if (blackWasExecuted && blackCapturingPawn == "BP" && blackOriginalSquare == "." && blackVictimSquare == ".")
            {
                Console.WriteLine("✓ Black en passant capture executed correctly");
            }
            else
            {
                Console.WriteLine($"✗ Black capture failed. Executed: {GetWasExecutedProperty(blackCaptureInfo)}, Capturing: {blackCapturingPawn}, Original: {blackOriginalSquare}, Victim: {blackVictimSquare}");
            }
            
            // Test 3: Invalid capture attempt should throw exception
            SetupInvalidCaptureScenario(board);
            
            bool exceptionThrown = false;
            try
            {
                ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            
            if (exceptionThrown)
            {
                Console.WriteLine("✓ Invalid capture attempt correctly throws exception");
            }
            else
            {
                Console.WriteLine("✗ Invalid capture attempt should throw exception");
            }
        }

        private static void TestRollbackEnPassantCapture()
        {
            Console.WriteLine("Testing RollbackEnPassantCapture method...");
            
            ChessBoard board = new ChessBoard();
            SetupValidCaptureScenario(board, "W");
            
            // Capture initial state
            string initialCapturingPawn = GetBoardPiece(board, 3, 4);
            string initialTarget = GetBoardPiece(board, 2, 5);
            string initialVictim = GetBoardPiece(board, 3, 5);
            
            // Execute capture
            var captureInfo = ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            // Verify capture was executed
            string afterCaptureCapturing = GetBoardPiece(board, 2, 5);
            string afterCaptureOriginal = GetBoardPiece(board, 3, 4);
            string afterCaptureVictim = GetBoardPiece(board, 3, 5);
            
            // Rollback the capture
            RollbackEnPassantCapture(board, captureInfo);
            
            // Verify rollback
            string afterRollbackCapturing = GetBoardPiece(board, 3, 4);
            string afterRollbackTarget = GetBoardPiece(board, 2, 5);
            string afterRollbackVictim = GetBoardPiece(board, 3, 5);
            
            if (initialCapturingPawn == "WP" && initialTarget == "." && initialVictim == "BP" &&
                afterCaptureCapturing == "WP" && afterCaptureOriginal == "." && afterCaptureVictim == "." &&
                afterRollbackCapturing == "WP" && afterRollbackTarget == "." && afterRollbackVictim == "BP" &&
                !GetWasExecutedProperty(captureInfo))
            {
                Console.WriteLine("✓ En passant capture rollback works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Rollback failed. Initial: {initialCapturingPawn}/{initialTarget}/{initialVictim}, After rollback: {afterRollbackCapturing}/{afterRollbackTarget}/{afterRollbackVictim}");
            }
        }

        private static void TestCanExecuteEnPassantCapture()
        {
            Console.WriteLine("Testing CanExecuteEnPassantCapture method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Valid capture scenario
            SetupValidCaptureScenario(board, "W");
            bool canExecuteValid = CanExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (canExecuteValid)
            {
                Console.WriteLine("✓ Valid capture scenario correctly identified");
            }
            else
            {
                Console.WriteLine("✗ Valid capture scenario not identified");
            }
            
            // Test 2: Invalid capture - no en passant opportunity
            ClearEnPassantOpportunity(board);
            bool canExecuteNoOpportunity = CanExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (!canExecuteNoOpportunity)
            {
                Console.WriteLine("✓ No opportunity scenario correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ No opportunity scenario incorrectly accepted");
            }
            
            // Test 3: Invalid capture - board boundaries
            SetupValidCaptureScenario(board, "W");
            bool canExecuteOutOfBounds = CanExecuteEnPassantCapture(board, 3, 4, -1, 5, "W");
            
            if (!canExecuteOutOfBounds)
            {
                Console.WriteLine("✓ Out of bounds scenario correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Out of bounds scenario incorrectly accepted");
            }
            
            // Test 4: Invalid capture - wrong piece type
            SetupValidCaptureScenario(board, "W");
            SetBoardPiece(board, 3, 4, "WR"); // Replace pawn with rook
            bool canExecuteWrongPiece = CanExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (!canExecuteWrongPiece)
            {
                Console.WriteLine("✓ Wrong piece type scenario correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Wrong piece type scenario incorrectly accepted");
            }
            
            // Test 5: Invalid capture - target square occupied
            SetupValidCaptureScenario(board, "W");
            SetBoardPiece(board, 2, 5, "BR"); // Block target square
            bool canExecuteBlocked = CanExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (!canExecuteBlocked)
            {
                Console.WriteLine("✓ Blocked target scenario correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Blocked target scenario incorrectly accepted");
            }
        }

        private static void TestAtomicBoardStateUpdates()
        {
            Console.WriteLine("Testing atomic board state updates...");
            
            ChessBoard board = new ChessBoard();
            SetupValidCaptureScenario(board, "W");
            
            // Capture board state before execution
            string[] boardStateBefore = CaptureFullBoardState(board);
            
            // Execute capture
            var captureInfo = ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            // Verify atomic update - exactly 3 squares should change
            string[] boardStateAfter = CaptureFullBoardState(board);
            int changedSquares = 0;
            
            for (int i = 0; i < 64; i++)
            {
                if (boardStateBefore[i] != boardStateAfter[i])
                {
                    changedSquares++;
                }
            }
            
            // Should be exactly 3 changes: capturing pawn moves, original square cleared, victim removed
            if (changedSquares == 3 && GetWasExecutedProperty(captureInfo))
            {
                Console.WriteLine("✓ Atomic board state update works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Atomic update failed. Changed squares: {changedSquares}, Expected: 3");
            }
            
            // Test rollback atomicity
            RollbackEnPassantCapture(board, captureInfo);
            string[] boardStateAfterRollback = CaptureFullBoardState(board);
            
            bool rollbackAtomic = true;
            for (int i = 0; i < 64; i++)
            {
                if (boardStateBefore[i] != boardStateAfterRollback[i])
                {
                    rollbackAtomic = false;
                    break;
                }
            }
            
            if (rollbackAtomic && !GetWasExecutedProperty(captureInfo))
            {
                Console.WriteLine("✓ Atomic rollback works correctly");
            }
            else
            {
                Console.WriteLine("✗ Atomic rollback failed");
            }
        }

        private static void TestErrorHandlingAndValidation()
        {
            Console.WriteLine("Testing error handling and validation...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Exception handling for invalid capture
            SetupInvalidCaptureScenario(board);
            
            bool properExceptionHandling = false;
            try
            {
                ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            }
            catch (InvalidOperationException ex)
            {
                properExceptionHandling = ex.Message.Contains("Invalid en passant capture attempt");
            }
            
            if (properExceptionHandling)
            {
                Console.WriteLine("✓ Proper exception handling for invalid captures");
            }
            else
            {
                Console.WriteLine("✗ Exception handling for invalid captures failed");
            }
            
            // Test 2: Boundary validation
            SetupValidCaptureScenario(board, "W");
            
            bool boundaryValidation = !CanExecuteEnPassantCapture(board, -1, 4, 2, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, -1, 2, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, 4, -1, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, 4, 2, -1, "W") &&
                                    !CanExecuteEnPassantCapture(board, 8, 4, 2, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, 8, 2, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, 4, 8, 5, "W") &&
                                    !CanExecuteEnPassantCapture(board, 3, 4, 2, 8, "W");
            
            if (boundaryValidation)
            {
                Console.WriteLine("✓ Boundary validation works correctly");
            }
            else
            {
                Console.WriteLine("✗ Boundary validation failed");
            }
            
            // Test 3: Piece validation
            SetupValidCaptureScenario(board, "W");
            SetBoardPiece(board, 3, 5, "."); // Remove victim pawn
            
            bool pieceValidation = !CanExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
            
            if (pieceValidation)
            {
                Console.WriteLine("✓ Piece validation works correctly");
            }
            else
            {
                Console.WriteLine("✗ Piece validation failed");
            }
        }

        private static void TestCaptureExecutionWithUserFeedback()
        {
            Console.WriteLine("Testing capture execution with user feedback...");
            
            ChessBoard board = new ChessBoard();
            
            // Capture console output to verify feedback messages
            var originalOut = Console.Out;
            var stringWriter = new System.IO.StringWriter();
            Console.SetOut(stringWriter);
            
            try
            {
                // Test White capture feedback
                SetupValidCaptureScenario(board, "W");
                ExecuteEnPassantCapture(board, 3, 4, 2, 5, "W");
                
                string whiteOutput = stringWriter.ToString();
                bool whiteFeedback = whiteOutput.Contains("En passant capture!") && 
                                   whiteOutput.Contains("W pawn captures Black pawn");
                
                // Clear and test Black capture feedback
                stringWriter.GetStringBuilder().Clear();
                SetupValidCaptureScenario(board, "B");
                ExecuteEnPassantCapture(board, 4, 3, 5, 2, "B");
                
                string blackOutput = stringWriter.ToString();
                bool blackFeedback = blackOutput.Contains("En passant capture!") && 
                                   blackOutput.Contains("B pawn captures White pawn");
                
                if (whiteFeedback && blackFeedback)
                {
                    Console.SetOut(originalOut);
                    Console.WriteLine("✓ User feedback messages work correctly");
                }
                else
                {
                    Console.SetOut(originalOut);
                    Console.WriteLine($"✗ User feedback failed. White: {whiteFeedback}, Black: {blackFeedback}");
                }
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        // Helper methods using reflection to access private methods and set up test scenarios
        private static object ExecuteEnPassantCapture(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("ExecuteEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static void RollbackEnPassantCapture(ChessBoard board, object captureInfo)
        {
            var method = typeof(ChessBoard).GetMethod("RollbackEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { captureInfo });
        }

        private static bool CanExecuteEnPassantCapture(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("CanExecuteEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static void SetEnPassantOpportunity(ChessBoard board, int pawnRow, int pawnCol)
        {
            var method = typeof(ChessBoard).GetMethod("SetEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { pawnRow, pawnCol });
        }

        private static void ClearEnPassantOpportunity(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("ClearEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, null);
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

        private static void SetupValidCaptureScenario(ChessBoard board, string color)
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
                // White pawn ready for en passant capture
                boardArray[3, 4] = "WP";
                boardArray[3, 5] = "BP"; // Black pawn that just moved two squares
                SetEnPassantOpportunity(board, 3, 5);
                SetCurrentTurn(board, "W");
            }
            else
            {
                // Black pawn ready for en passant capture
                boardArray[4, 3] = "BP";
                boardArray[4, 2] = "WP"; // White pawn that just moved two squares
                SetEnPassantOpportunity(board, 4, 2);
                SetCurrentTurn(board, "B");
            }
        }

        private static void SetupInvalidCaptureScenario(ChessBoard board)
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
            
            // Set up invalid scenario - no en passant opportunity
            boardArray[3, 4] = "WP";
            boardArray[3, 5] = "BP";
            // Don't set en passant opportunity
            ClearEnPassantOpportunity(board);
            SetCurrentTurn(board, "W");
        }

        private static string[] CaptureFullBoardState(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            string[] state = new string[64];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    state[row * 8 + col] = boardArray[row, col];
                }
            }
            return state;
        }

        private static bool GetWasExecutedProperty(object captureInfo)
        {
            if (captureInfo == null) return false;
            var property = captureInfo.GetType().GetProperty("WasExecuted");
            return property != null && (bool)property.GetValue(captureInfo);
        }
    }

    // Extension to access private class properties for testing
    public static class CaptureInfoExtensions
    {
        public static bool WasExecuted(this object captureInfo)
        {
            if (captureInfo == null) return false;
            var property = captureInfo.GetType().GetProperty("WasExecuted");
            return property != null && (bool)property.GetValue(captureInfo);
        }
    }
}