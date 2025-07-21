using System;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantMoveDetection
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant Move Detection Logic ===");
            
            TestIsEnPassantCapture();
            TestIsValidEnPassantMove();
            TestCreatesEnPassantOpportunity();
            TestUpdateEnPassantState();
            TestEnPassantDetectionEdgeCases();
            
            Console.WriteLine("=== En Passant Move Detection Tests Completed ===");
        }

        private static void TestIsEnPassantCapture()
        {
            Console.WriteLine("Testing IsEnPassantCapture method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Valid en passant capture scenario for White
            SetupEnPassantScenario(board, "W", 3, 4, 3, 5); // White pawn at (3,4), Black pawn at (3,5)
            SetEnPassantOpportunity(board, 3, 5); // Black pawn just moved two squares
            
            bool validCapture = IsEnPassantCapture(board, 3, 4, 2, 5, "W");
            if (validCapture)
            {
                Console.WriteLine("✓ Valid White en passant capture detected correctly");
            }
            else
            {
                Console.WriteLine("✗ Valid White en passant capture not detected");
            }
            
            // Test 2: Valid en passant capture scenario for Black
            SetupEnPassantScenario(board, "B", 4, 3, 4, 2); // Black pawn at (4,3), White pawn at (4,2)
            SetEnPassantOpportunity(board, 4, 2); // White pawn just moved two squares
            
            bool validBlackCapture = IsEnPassantCapture(board, 4, 3, 5, 2, "B");
            if (validBlackCapture)
            {
                Console.WriteLine("✓ Valid Black en passant capture detected correctly");
            }
            else
            {
                Console.WriteLine("✗ Valid Black en passant capture not detected");
            }
            
            // Test 3: Invalid capture - no en passant opportunity
            ClearEnPassantOpportunity(board);
            bool invalidCapture = IsEnPassantCapture(board, 3, 4, 2, 5, "W");
            if (!invalidCapture)
            {
                Console.WriteLine("✓ Invalid capture (no opportunity) correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid capture (no opportunity) incorrectly accepted");
            }
            
            // Test 4: Invalid capture - wrong piece type
            SetupEnPassantScenario(board, "W", 3, 4, 3, 5);
            SetBoardPiece(board, 3, 4, "WR"); // Place rook instead of pawn
            SetEnPassantOpportunity(board, 3, 5);
            
            bool wrongPiece = IsEnPassantCapture(board, 3, 4, 2, 5, "W");
            if (!wrongPiece)
            {
                Console.WriteLine("✓ Invalid capture (wrong piece) correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid capture (wrong piece) incorrectly accepted");
            }
        }

        private static void TestIsValidEnPassantMove()
        {
            Console.WriteLine("Testing IsValidEnPassantMove method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Complete valid en passant scenario
            SetupValidEnPassantScenario(board, "W");
            bool validMove = IsValidEnPassantMove(board, 3, 4, 2, 5, "W");
            if (validMove)
            {
                Console.WriteLine("✓ Complete valid en passant move validated correctly");
            }
            else
            {
                Console.WriteLine("✗ Complete valid en passant move validation failed");
            }
            
            // Test 2: Invalid - target square occupied
            SetupValidEnPassantScenario(board, "W");
            SetBoardPiece(board, 2, 5, "BR"); // Block target square
            bool blockedTarget = IsValidEnPassantMove(board, 3, 4, 2, 5, "W");
            if (!blockedTarget)
            {
                Console.WriteLine("✓ Invalid move (blocked target) correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid move (blocked target) incorrectly accepted");
            }
            
            // Test 3: Invalid - no victim pawn
            SetupValidEnPassantScenario(board, "W");
            SetBoardPiece(board, 3, 5, "."); // Remove victim pawn
            bool noVictim = IsValidEnPassantMove(board, 3, 4, 2, 5, "W");
            if (!noVictim)
            {
                Console.WriteLine("✓ Invalid move (no victim) correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid move (no victim) incorrectly accepted");
            }
            
            // Test 4: Invalid - wrong rank for capturing pawn
            SetupValidEnPassantScenario(board, "W");
            SetBoardPiece(board, 3, 4, "."); // Remove pawn from correct rank
            SetBoardPiece(board, 2, 4, "WP"); // Place on wrong rank
            bool wrongRank = IsValidEnPassantMove(board, 2, 4, 2, 5, "W");
            if (!wrongRank)
            {
                Console.WriteLine("✓ Invalid move (wrong rank) correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Invalid move (wrong rank) incorrectly accepted");
            }
        }

        private static void TestCreatesEnPassantOpportunity()
        {
            Console.WriteLine("Testing CreatesEnPassantOpportunity method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Valid two-square pawn move by White
            SetupTwoSquareMoveScenario(board, "W");
            bool whiteCreates = CreatesEnPassantOpportunity(board, 6, 3, 4, 3, "W");
            if (whiteCreates)
            {
                Console.WriteLine("✓ White two-square move creates en passant opportunity");
            }
            else
            {
                Console.WriteLine("✗ White two-square move should create en passant opportunity");
            }
            
            // Test 2: Valid two-square pawn move by Black
            SetupTwoSquareMoveScenario(board, "B");
            bool blackCreates = CreatesEnPassantOpportunity(board, 1, 3, 3, 3, "B");
            if (blackCreates)
            {
                Console.WriteLine("✓ Black two-square move creates en passant opportunity");
            }
            else
            {
                Console.WriteLine("✗ Black two-square move should create en passant opportunity");
            }
            
            // Test 3: Invalid - one square move
            SetupTwoSquareMoveScenario(board, "W");
            bool oneSquare = CreatesEnPassantOpportunity(board, 6, 3, 5, 3, "W");
            if (!oneSquare)
            {
                Console.WriteLine("✓ One-square move correctly doesn't create opportunity");
            }
            else
            {
                Console.WriteLine("✗ One-square move incorrectly creates opportunity");
            }
            
            // Test 4: Invalid - not from starting rank
            SetupTwoSquareMoveScenario(board, "W");
            SetBoardPiece(board, 6, 3, "."); // Remove from starting rank
            SetBoardPiece(board, 5, 3, "WP"); // Place on different rank
            bool wrongStart = CreatesEnPassantOpportunity(board, 5, 3, 3, 3, "W");
            if (!wrongStart)
            {
                Console.WriteLine("✓ Move from wrong rank correctly doesn't create opportunity");
            }
            else
            {
                Console.WriteLine("✗ Move from wrong rank incorrectly creates opportunity");
            }
            
            // Test 5: Invalid - diagonal move
            SetupTwoSquareMoveScenario(board, "W");
            bool diagonal = CreatesEnPassantOpportunity(board, 6, 3, 4, 4, "W");
            if (!diagonal)
            {
                Console.WriteLine("✓ Diagonal move correctly doesn't create opportunity");
            }
            else
            {
                Console.WriteLine("✗ Diagonal move incorrectly creates opportunity");
            }
        }

        private static void TestUpdateEnPassantState()
        {
            Console.WriteLine("Testing UpdateEnPassantState method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: Two-square pawn move should set en passant opportunity
            SetupTwoSquareMoveScenario(board, "W");
            UpdateEnPassantState(board, 6, 3, 4, 3, "P", "W");
            
            bool hasOpportunity = HasEnPassantOpportunity(board);
            var target = GetEnPassantTarget(board);
            var victim = GetEnPassantVictim(board);
            
            if (hasOpportunity && target.row == 5 && target.col == 3 && victim.row == 4 && victim.col == 3)
            {
                Console.WriteLine("✓ Two-square pawn move correctly sets en passant state");
            }
            else
            {
                Console.WriteLine($"✗ En passant state incorrect. Has: {hasOpportunity}, Target: ({target.row},{target.col}), Victim: ({victim.row},{victim.col})");
            }
            
            // Test 2: Non-pawn move should clear en passant opportunity
            SetEnPassantOpportunity(board, 4, 3); // Set an opportunity first
            UpdateEnPassantState(board, 7, 0, 5, 0, "R", "W"); // Rook move
            
            bool clearedOpportunity = !HasEnPassantOpportunity(board);
            if (clearedOpportunity)
            {
                Console.WriteLine("✓ Non-pawn move correctly clears en passant opportunity");
            }
            else
            {
                Console.WriteLine("✗ Non-pawn move should clear en passant opportunity");
            }
            
            // Test 3: One-square pawn move should clear en passant opportunity
            SetEnPassantOpportunity(board, 4, 3); // Set an opportunity first
            UpdateEnPassantState(board, 6, 2, 5, 2, "P", "W"); // One-square pawn move
            
            bool clearedByPawn = !HasEnPassantOpportunity(board);
            if (clearedByPawn)
            {
                Console.WriteLine("✓ One-square pawn move correctly clears en passant opportunity");
            }
            else
            {
                Console.WriteLine("✗ One-square pawn move should clear en passant opportunity");
            }
        }

        private static void TestEnPassantDetectionEdgeCases()
        {
            Console.WriteLine("Testing en passant detection edge cases...");
            
            ChessBoard board = new ChessBoard();
            
            // Test 1: En passant capture in wrong direction
            SetupValidEnPassantScenario(board, "W");
            bool wrongDirection = IsEnPassantCapture(board, 3, 4, 4, 5, "W"); // Moving backward
            if (!wrongDirection)
            {
                Console.WriteLine("✓ Wrong direction capture correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Wrong direction capture incorrectly accepted");
            }
            
            // Test 2: En passant with non-adjacent pawn
            SetupValidEnPassantScenario(board, "W");
            SetBoardPiece(board, 3, 4, "."); // Remove adjacent pawn
            SetBoardPiece(board, 3, 2, "WP"); // Place non-adjacent pawn
            bool nonAdjacent = IsEnPassantCapture(board, 3, 2, 2, 5, "W");
            if (!nonAdjacent)
            {
                Console.WriteLine("✓ Non-adjacent pawn capture correctly rejected");
            }
            else
            {
                Console.WriteLine("✗ Non-adjacent pawn capture incorrectly accepted");
            }
            
            // Test 3: En passant opportunity creation at board edges
            SetupTwoSquareMoveScenario(board, "W");
            SetBoardPiece(board, 6, 3, "."); // Remove center pawn
            SetBoardPiece(board, 6, 0, "WP"); // Place at edge
            bool edgeOpportunity = CreatesEnPassantOpportunity(board, 6, 0, 4, 0, "W");
            if (edgeOpportunity)
            {
                Console.WriteLine("✓ En passant opportunity creation works at board edges");
            }
            else
            {
                Console.WriteLine("✗ En passant opportunity creation should work at board edges");
            }
            
            // Test 4: Multiple consecutive two-square moves
            SetupTwoSquareMoveScenario(board, "W");
            UpdateEnPassantState(board, 6, 3, 4, 3, "P", "W"); // First two-square move
            UpdateEnPassantState(board, 6, 4, 4, 4, "P", "W"); // Second two-square move
            
            var finalTarget = GetEnPassantTarget(board);
            var finalVictim = GetEnPassantVictim(board);
            
            if (finalTarget.col == 4 && finalVictim.col == 4)
            {
                Console.WriteLine("✓ Multiple two-square moves correctly update en passant state");
            }
            else
            {
                Console.WriteLine($"✗ Multiple moves state incorrect. Target col: {finalTarget.col}, Victim col: {finalVictim.col}");
            }
        }

        // Helper methods using reflection to access private methods and set up test scenarios
        private static bool IsEnPassantCapture(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsEnPassantCapture", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static bool IsValidEnPassantMove(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("IsValidEnPassantMove", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static bool CreatesEnPassantOpportunity(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var method = typeof(ChessBoard).GetMethod("CreatesEnPassantOpportunity", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, color });
        }

        private static void UpdateEnPassantState(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol, string pieceType, string color)
        {
            var method = typeof(ChessBoard).GetMethod("UpdateEnPassantState", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(board, new object[] { fromRow, fromCol, toRow, toCol, pieceType, color });
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

        private static void SetBoardPiece(ChessBoard board, int row, int col, string piece)
        {
            var boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            boardArray[row, col] = piece;
        }

        private static void SetupEnPassantScenario(ChessBoard board, string color, int pawnRow, int pawnCol, int victimRow, int victimCol)
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
            
            // Set up pawns
            boardArray[pawnRow, pawnCol] = color + "P";
            string opponentColor = (color == "W") ? "B" : "W";
            boardArray[victimRow, victimCol] = opponentColor + "P";
        }

        private static void SetupValidEnPassantScenario(ChessBoard board, string color)
        {
            if (color == "W")
            {
                SetupEnPassantScenario(board, "W", 3, 4, 3, 5); // White pawn at (3,4), Black victim at (3,5)
                SetEnPassantOpportunity(board, 3, 5); // Black pawn just moved two squares
            }
            else
            {
                SetupEnPassantScenario(board, "B", 4, 3, 4, 2); // Black pawn at (4,3), White victim at (4,2)
                SetEnPassantOpportunity(board, 4, 2); // White pawn just moved two squares
            }
        }

        private static void SetupTwoSquareMoveScenario(ChessBoard board, string color)
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
            
            // Set up pawn on starting rank
            if (color == "W")
            {
                boardArray[6, 3] = "WP"; // White pawn on starting rank
            }
            else
            {
                boardArray[1, 3] = "BP"; // Black pawn on starting rank
            }
        }
    }
}