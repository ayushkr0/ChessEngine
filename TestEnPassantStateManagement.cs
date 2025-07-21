using System;
using System.Reflection;

namespace ChessEngine
{
    public class TestEnPassantStateManagement
    {
        public static void Main()
        {
            RunAllTests();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        public static void RunAllTests()
        {
            Console.WriteLine("=== Testing En Passant State Management ===");
            
            TestSetEnPassantOpportunity();
            TestClearEnPassantOpportunity();
            TestHasEnPassantOpportunity();
            TestIsValidEnPassantTarget();
            TestGetEnPassantVictimPosition();
            
            Console.WriteLine("=== En Passant State Management Tests Completed ===");
        }

        private static void TestSetEnPassantOpportunity()
        {
            Console.WriteLine("Testing SetEnPassantOpportunity method...");
            
            ChessBoard board = new ChessBoard();
            
            // Test White pawn two-square move (creates en passant opportunity on rank 3)
            SetEnPassantOpportunity(board, 3, 4); // White pawn moved to row 3, col 4
            
            var enPassantTarget = GetEnPassantTarget(board);
            var enPassantVictim = GetEnPassantVictim(board);
            
            if (enPassantTarget.row == 2 && enPassantTarget.col == 4 &&
                enPassantVictim.row == 3 && enPassantVictim.col == 4)
            {
                Console.WriteLine("✓ White pawn en passant opportunity set correctly");
            }
            else
            {
                Console.WriteLine($"✗ White pawn test failed. Target: ({enPassantTarget.row}, {enPassantTarget.col}), Victim: ({enPassantVictim.row}, {enPassantVictim.col})");
            }
            
            // Test Black pawn two-square move (creates en passant opportunity on rank 6)
            SetEnPassantOpportunity(board, 4, 2); // Black pawn moved to row 4, col 2
            
            enPassantTarget = GetEnPassantTarget(board);
            enPassantVictim = GetEnPassantVictim(board);
            
            if (enPassantTarget.row == 5 && enPassantTarget.col == 2 &&
                enPassantVictim.row == 4 && enPassantVictim.col == 2)
            {
                Console.WriteLine("✓ Black pawn en passant opportunity set correctly");
            }
            else
            {
                Console.WriteLine($"✗ Black pawn test failed. Target: ({enPassantTarget.row}, {enPassantTarget.col}), Victim: ({enPassantVictim.row}, {enPassantVictim.col})");
            }
        }

        private static void TestClearEnPassantOpportunity()
        {
            Console.WriteLine("Testing ClearEnPassantOpportunity method...");
            
            ChessBoard board = new ChessBoard();
            
            // First set an en passant opportunity
            SetEnPassantOpportunity(board, 3, 1);
            
            // Verify it's set
            bool hasOpportunityBefore = HasEnPassantOpportunity(board);
            
            // Clear the opportunity
            ClearEnPassantOpportunity(board);
            
            // Verify it's cleared
            bool hasOpportunityAfter = HasEnPassantOpportunity(board);
            var enPassantTarget = GetEnPassantTarget(board);
            var enPassantVictim = GetEnPassantVictim(board);
            
            if (hasOpportunityBefore && !hasOpportunityAfter &&
                enPassantTarget.row == -1 && enPassantTarget.col == -1 &&
                enPassantVictim.row == -1 && enPassantVictim.col == -1)
            {
                Console.WriteLine("✓ En passant opportunity cleared correctly");
            }
            else
            {
                Console.WriteLine($"✗ Clear test failed. Before: {hasOpportunityBefore}, After: {hasOpportunityAfter}");
            }
        }

        private static void TestHasEnPassantOpportunity()
        {
            Console.WriteLine("Testing HasEnPassantOpportunity method...");
            
            ChessBoard board = new ChessBoard();
            
            // Initially should have no opportunity
            bool initialState = HasEnPassantOpportunity(board);
            
            // Set an opportunity
            SetEnPassantOpportunity(board, 3, 5);
            bool afterSet = HasEnPassantOpportunity(board);
            
            // Clear the opportunity
            ClearEnPassantOpportunity(board);
            bool afterClear = HasEnPassantOpportunity(board);
            
            if (!initialState && afterSet && !afterClear)
            {
                Console.WriteLine("✓ HasEnPassantOpportunity works correctly");
            }
            else
            {
                Console.WriteLine($"✗ HasEnPassantOpportunity test failed. Initial: {initialState}, After set: {afterSet}, After clear: {afterClear}");
            }
        }

        private static void TestIsValidEnPassantTarget()
        {
            Console.WriteLine("Testing IsValidEnPassantTarget method...");
            
            ChessBoard board = new ChessBoard();
            
            // Set en passant opportunity at (2, 3)
            SetEnPassantOpportunity(board, 3, 3);
            
            // Test valid target
            bool validTarget = IsValidEnPassantTarget(board, 2, 3);
            
            // Test invalid targets
            bool invalidTarget1 = IsValidEnPassantTarget(board, 2, 4); // Wrong column
            bool invalidTarget2 = IsValidEnPassantTarget(board, 3, 3); // Wrong row
            bool invalidTarget3 = IsValidEnPassantTarget(board, 1, 1); // Completely wrong
            
            // Clear opportunity and test
            ClearEnPassantOpportunity(board);
            bool noOpportunity = IsValidEnPassantTarget(board, 2, 3);
            
            if (validTarget && !invalidTarget1 && !invalidTarget2 && !invalidTarget3 && !noOpportunity)
            {
                Console.WriteLine("✓ IsValidEnPassantTarget works correctly");
            }
            else
            {
                Console.WriteLine($"✗ IsValidEnPassantTarget test failed. Valid: {validTarget}, Invalid tests: {invalidTarget1}, {invalidTarget2}, {invalidTarget3}, No opportunity: {noOpportunity}");
            }
        }

        private static void TestGetEnPassantVictimPosition()
        {
            Console.WriteLine("Testing GetEnPassantVictimPosition method...");
            
            ChessBoard board = new ChessBoard();
            
            // Set en passant opportunity
            SetEnPassantOpportunity(board, 4, 6); // Black pawn at (4, 6)
            
            var victimPosition = GetEnPassantVictimPosition(board);
            
            if (victimPosition.row == 4 && victimPosition.col == 6)
            {
                Console.WriteLine("✓ GetEnPassantVictimPosition works correctly");
            }
            else
            {
                Console.WriteLine($"✗ GetEnPassantVictimPosition test failed. Expected: (4, 6), Got: ({victimPosition.row}, {victimPosition.col})");
            }
        }

        // Helper methods using reflection to access private methods and fields
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

        private static bool IsValidEnPassantTarget(ChessBoard board, int toRow, int toCol)
        {
            var method = typeof(ChessBoard).GetMethod("IsValidEnPassantTarget", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(board, new object[] { toRow, toCol });
        }

        private static (int row, int col) GetEnPassantVictimPosition(ChessBoard board)
        {
            var method = typeof(ChessBoard).GetMethod("GetEnPassantVictimPosition", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return ((int, int))method.Invoke(board, null);
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
    }
}