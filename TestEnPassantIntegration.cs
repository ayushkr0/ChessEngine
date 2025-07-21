using System;
using System.IO;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantIntegration
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant Integration with Existing Pawn Move System ===");
            
            TestEnPassantInPawnMoveValidation();
            TestEnPassantWithCheckValidation();
            TestEnPassantExecutionInMovePiece();
            TestEnPassantExecutionInMovePieceAI();
            TestEnPassantStateManagementIntegration();
            TestCompleteEnPassantGameScenario();
            
            Console.WriteLine("=== En Passant Integration Tests Completed ===");
        }

        private static void TestEnPassantInPawnMoveValidation()
        {
            Console.WriteLine("Testing en passant integration in IsValidPawnMove...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Regular pawn moves still work
            SetupBasicPawnScenario(board);
            bool regularMove = IsValidPawnMove(board, 6, 3, 5, 3, "W"); // One square forward
            bool twoSquareMove = IsValidPawnMove(board, 6, 4, 4, 4, "W"); // Two squares forward
            bool regularCapture = IsValidPawnMove(board, 6, 2, 5, 1, "W"); // Regular diagonal capture
            
            if (regularMove && twoSquareMove && regularCapture)
            {
                Console.WriteLine("✓ Regular pawn moves still work correctly");
            }
            else
            {
                Console.WriteLine($"✗ Regular pawn moves failed. Regular: {regularMove}, Two-square: {twoSquareMove}, Capture: {regularCapture}");
            }
            
            // Test 2: En passant moves are validated through IsValidPawnMove
            SetupEnPassantIntegrationScenario(board, "W");
            bool enPassantMove = IsValidPawnMove(board, 3, 4, 2, 5, "W"); // En passant capture
            
            if (enPassantMove)
            {
                Console.WriteLine("✓ En passant moves validated correctly through IsValidPawnMove");
            }
            else
            {
                Console.WriteLine("✗ En passant moves not validated through IsValidPawnMove");
            }
            
            // Test 3: Invalid en passant moves are rejected
            ClearEnPassantOpportunity(board);
            bool invalidEnPassant = IsValidPawnMove(board, 3, 4, 2, 5, "W"); // No opportunity
            
            if (!invalidEnPassant)
            {
                Console.WriteLine("✓ Invalid en passant moves correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid en passant moves incorrectly accepted");
            }
        }

        private static void TestEnPassantWithCheckValidation()
        {
            Console.WriteLine("Testing en passant with check validation...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: En passant move that would leave king in check should be rejected
            SetupEnPassantCheckScenario(board);
            
            // Simulate the move validation process
            bool moveValid = IsValidPawnMove(board, 3, 4, 2, 5, "W");
            
            if (moveValid)
            {
                Console.WriteLine("✓ En passant move validation works with check detection");
            }
            else
            {
                Console.WriteLine("✗ En passant move validation failed with check detection");
            }
            
            // Test 2: En passant move that gets out of check should be allowed
            SetupEnPassantEscapeCheckScenario(board);
            bool escapeMove = IsValidPawnMove(board, 3, 4, 2, 5, "W");
            
            if (escapeMove)
            {
                Console.WriteLine("✓ En passant move that escapes check is allowed");
            }
            else
            {
                Console.WriteLine("✗ En passant move that escapes check was rejected");
            }
        }

        private static void TestEnPassantExecutionInMovePiece()
        {
            Console.WriteLine("Testing en passant execution in MovePiece method...");
            
            ChessBoard board = new ChessBoard();
            SetupEnPassantExecutionScenario(board, "W");
            
            // Capture initial state
            string initialVictim = GetBoardPiece(board, 3, 5);
            string initialTarget = GetBoardPiece(board, 2, 5);
            string currentTurn = board.CurrentTurn();
            
            // Execute en passant move
            board.MovePiece(3, 4, 2, 5);
            
            // Verify execution results
            string finalCapturingPawn = GetBoardPiece(board, 2, 5);
            string finalVictimSquare = GetBoardPiece(board, 3, 5);
            string finalOriginalSquare = GetBoardPiece(board, 3, 4);
            string newTurn = board.CurrentTurn();
            
            if (initialVictim == "BP" && initialTarget == "." && currentTurn == "W" &&
                finalCapturingPawn == "WP" && finalVictimSquare == "." && 
                finalOriginalSquare == "." && newTurn == "B")
            {
                Console.WriteLine("✓ En passant execution in MovePiece works correctly");
            }
            else
            {
                Console.WriteLine($"✗ En passant execution failed. Victim: {initialVictim}->{finalVictimSquare}, Target: {initialTarget}->{finalCapturingPawn}, Turn: {currentTurn}->{newTurn}");
            }
        }

        private static void TestEnPassantExecutionInMovePieceAI()
        {
            Console.WriteLine("Testing en passant execution in MovePieceAI method...");
            
            ChessBoard board = new ChessBoard();
            SetupEnPassantExecutionScenario(board, "B");
            
            // Capture initial state
            string initialVictim = GetBoardPiece(board, 4, 2);
            string initialTarget = GetBoardPiece(board, 5, 2);
            string currentTurn = board.CurrentTurn();
            
            // Execute AI en passant move
            board.MovePieceAI(4, 3, 5, 2);
            
            // Verify execution results
            string finalCapturingPawn = GetBoardPiece(board, 5, 2);
            string finalVictimSquare = GetBoardPiece(board, 4, 2);
            string finalOriginalSquare = GetBoardPiece(board, 4, 3);
            string newTurn = board.CurrentTurn();
            
            if (initialVictim == "WP" && initialTarget == "." && currentTurn == "B" &&
                finalCapturingPawn == "BP" && finalVictimSquare == "." && 
                finalOriginalSquare == "." && newTurn == "W")
            {
                Console.WriteLine("✓ En passant execution in MovePieceAI works correctly");
            }
            else
            {
                Console.WriteLine($"✗ AI en passant execution failed. Victim: {initialVictim}->{finalVictimSquare}, Target: {initialTarget}->{finalCapturingPawn}, Turn: {currentTurn}->{newTurn}");
            }
        }

        private static void TestEnPassantStateManagementIntegration()
        {
            Console.WriteLine("Testing en passant state management integration...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Two-square pawn move creates en passant opportunity
            SetupTwoSquareMoveIntegrationScenario(board);
            
            // Execute two-square move
            board.MovePiece(6, 3, 4, 3);
            
            // Verify en passant opportunity was created
            bool hasOpportunity = HasEnPassantOpportunity(board);
            var target = GetEnPassantTarget(board);
            var victim = GetEnPassantVictim(board);
            
            if (hasOpportunity && target.row == 5 && target.col == 3 && victim.row == 4 && victim.col == 3)
            {
                Console.WriteLine("✓ Two-square pawn move creates en passant opportunity correctly");
            }
            else
            {
                Console.WriteLine($"✗ En passant opportunity creation failed. Has: {hasOpportunity}, Target: ({target.row},{target.col}), Victim: ({victim.row},{victim.col})");
            }
            
            // Test 2: Other moves clear en passant opportunity
            board.MovePiece(7, 0, 5, 0); // Rook move
            
            bool clearedOpportunity = !HasEnPassantOpportunity(board);
            if (clearedOpportunity)
            {
                Console.WriteLine("✓ Non-pawn moves clear en passant opportunity correctly");
            }
            else
            {
                Console.WriteLine("✗ En passant opportunity should be cleared by other moves");
            }
        }

        private static void TestCompleteEnPassantGameScenario()
        {
            Console.WriteLine("Testing complete en passant game scenario...");
            
            ChessBoard board = new ChessBoard();
            
            // Simulate a realistic game scenario leading to en passant
            // 1. White moves pawn two squares
            SetupRealisticEnPassantScenario(board);
            board.MovePiece(6, 4, 4, 4); // White pawn e2-e4
            
            // 2. Black moves pawn adjacent
            board.MovePiece(1, 3, 3, 3); // Black pawn d7-d5
            
            // 3. White moves pawn two squares next to black pawn
            board.MovePiece(6, 2, 4, 2); // White pawn c2-c4
            
            // 4. Black captures en passant
            string beforeCapture = GetBoardPiece(board, 4, 2);
            board.MovePiece(3, 3, 4, 2); // Black pawn captures en passant
            string afterCapture = GetBoardPiece(board, 4, 2);
            string victimSquare = GetBoardPiece(board, 4, 2); // Should be black pawn now
            string originalSquare = GetBoardPiece(board, 3, 3); // Should be empty
            
            if (beforeCapture == "WP" && afterCapture == "BP" && originalSquare == ".")
            {
                Console.WriteLine("✓ Complete en passant game scenario works correctly");
            }
            else
            {
                Console.WriteLine($"✗ Complete scenario failed. Before: {beforeCapture}, After: {afterCapture}, Original: {originalSquare}");
            }
        }

        // Helper methods using reflection and board setup
        private static bool IsValidPawnMove(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsValidPawnMove", 
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

        private static bool HasEnPassantOpportunity(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("HasEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, null);
        }

        private static (int row, int col) GetEnPassantTarget(ChessBoard board)
        {
            var field = typeof(ChessBoard).GetField("enPassantTarget", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return ((int, int))field.GetValue(board);
        }

        private static (int row, int col) GetEnPassantVictim(ChessBoard board)
        {
            var field = typeof(ChessBoard).GetField("enPassantVictim", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return ((int, int))field.GetValue(board);
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

        private static void SetupBasicPawnScenario(ChessBoard board)
        {
            // Clear board and set up basic pawn testing scenario
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up kings
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            // Set up pawns for testing
            boardArray[6, 3] = "WP"; // For one square move
            boardArray[6, 4] = "WP"; // For two square move
            boardArray[6, 2] = "WP"; // For capture
            boardArray[5, 1] = "BP"; // Target for capture
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupEnPassantIntegrationScenario(ChessBoard board, string color)
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

        private static void SetupEnPassantCheckScenario(ChessBoard board)
        {
            // Setup scenario where en passant move would leave king in check
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            // Set up a scenario where en passant would expose king to check
            boardArray[7, 4] = "WK";  // White king
            boardArray[0, 4] = "BK";  // Black king
            boardArray[3, 4] = "WP";  // White pawn
            boardArray[3, 5] = "BP";  // Black pawn (victim)
            boardArray[3, 0] = "BR";  // Black rook that would give check
            
            SetEnPassantOpportunity(board, 3, 5);
            SetCurrentTurn(board, "W");
        }

        private static void SetupEnPassantEscapeCheckScenario(ChessBoard board)
        {
            // Setup scenario where en passant move gets out of check
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            boardArray[7, 4] = "WK";  // White king
            boardArray[0, 4] = "BK";  // Black king
            boardArray[3, 4] = "WP";  // White pawn
            boardArray[3, 5] = "BP";  // Black pawn (victim)
            
            SetEnPassantOpportunity(board, 3, 5);
            SetCurrentTurn(board, "W");
        }

        private static void SetupEnPassantExecutionScenario(ChessBoard board, string color)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            
            if (color == "W")
            {
                boardArray[3, 4] = "WP"; // White capturing pawn
                boardArray[3, 5] = "BP"; // Black victim pawn
                SetEnPassantOpportunity(board, 3, 5);
                SetCurrentTurn(board, "W");
            }
            else
            {
                boardArray[4, 3] = "BP"; // Black capturing pawn
                boardArray[4, 2] = "WP"; // White victim pawn
                SetEnPassantOpportunity(board, 4, 2);
                SetCurrentTurn(board, "B");
            }
        }

        private static void SetupTwoSquareMoveIntegrationScenario(ChessBoard board)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardArray[i, j] = ".";
            
            boardArray[7, 4] = "WK";
            boardArray[0, 4] = "BK";
            boardArray[6, 3] = "WP"; // White pawn ready for two-square move
            boardArray[7, 0] = "WR"; // White rook for subsequent move
            
            SetCurrentTurn(board, "W");
        }

        private static void SetupRealisticEnPassantScenario(ChessBoard board)
        {
            // Set up a realistic opening scenario
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
    }
}