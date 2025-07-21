using System;
using System.Reflection;

namespace ChessEngine
{
    public class PromotionTest
    {
        public static void RunTests()
        {
            Console.WriteLine("Running Pawn Promotion Detection Tests...");
            
            var board = new ChessBoard();
            
            // Test RequiresPromotion method using reflection
            var method = typeof(ChessBoard).GetMethod("RequiresPromotion", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test 1: White pawn reaching rank 8 (row 0)
            bool result1 = (bool)method.Invoke(board, new object[] { 0, "W" });
            Console.WriteLine($"Test 1 - White pawn to rank 8: {(result1 ? "PASS" : "FAIL")} (Expected: True, Got: {result1})");
            
            // Test 2: Black pawn reaching rank 1 (row 7)
            bool result2 = (bool)method.Invoke(board, new object[] { 7, "B" });
            Console.WriteLine($"Test 2 - Black pawn to rank 1: {(result2 ? "PASS" : "FAIL")} (Expected: True, Got: {result2})");
            
            // Test 3: White pawn not on promotion rank
            bool result3 = (bool)method.Invoke(board, new object[] { 1, "W" });
            Console.WriteLine($"Test 3 - White pawn to rank 7: {(!result3 ? "PASS" : "FAIL")} (Expected: False, Got: {result3})");
            
            // Test 4: Black pawn not on promotion rank
            bool result4 = (bool)method.Invoke(board, new object[] { 6, "B" });
            Console.WriteLine($"Test 4 - Black pawn to rank 2: {(!result4 ? "PASS" : "FAIL")} (Expected: False, Got: {result4})");
            
            // Test 5: White pawn on middle rank
            bool result5 = (bool)method.Invoke(board, new object[] { 4, "W" });
            Console.WriteLine($"Test 5 - White pawn to rank 4: {(!result5 ? "PASS" : "FAIL")} (Expected: False, Got: {result5})");
            
            // Test 6: Black pawn on middle rank
            bool result6 = (bool)method.Invoke(board, new object[] { 3, "B" });
            Console.WriteLine($"Test 6 - Black pawn to rank 5: {(!result6 ? "PASS" : "FAIL")} (Expected: False, Got: {result6})");
            
            Console.WriteLine("Promotion detection tests completed.");
            Console.WriteLine();
            
            // Run piece selection validation tests
            RunPieceSelectionTests();
        }
        
        private static void RunPieceSelectionTests()
        {
            Console.WriteLine("Running Piece Selection Validation Tests...");
            
            var board = new ChessBoard();
            
            // Test IsValidPromotionChoice method using reflection
            var method = typeof(ChessBoard).GetMethod("IsValidPromotionChoice", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test valid choices (uppercase)
            TestPromotionChoice(board, method, "Q", true, 'Q', "Valid Queen choice (uppercase)");
            TestPromotionChoice(board, method, "R", true, 'R', "Valid Rook choice (uppercase)");
            TestPromotionChoice(board, method, "B", true, 'B', "Valid Bishop choice (uppercase)");
            TestPromotionChoice(board, method, "N", true, 'N', "Valid Knight choice (uppercase)");
            
            // Test valid choices (lowercase - should be handled by ToUpper in GetPromotionChoice)
            TestPromotionChoice(board, method, "q", true, 'Q', "Valid Queen choice (lowercase)");
            TestPromotionChoice(board, method, "r", true, 'R', "Valid Rook choice (lowercase)");
            TestPromotionChoice(board, method, "b", true, 'B', "Valid Bishop choice (lowercase)");
            TestPromotionChoice(board, method, "n", true, 'N', "Valid Knight choice (lowercase)");
            
            // Test valid choices with extra characters (should take first character)
            TestPromotionChoice(board, method, "Queen", true, 'Q', "Valid Queen with extra text");
            TestPromotionChoice(board, method, "Rook", true, 'R', "Valid Rook with extra text");
            TestPromotionChoice(board, method, "Bishop", true, 'B', "Valid Bishop with extra text");
            TestPromotionChoice(board, method, "Knight", true, 'N', "Valid Knight with extra text");
            
            // Test invalid choices
            TestPromotionChoice(board, method, "K", false, '\0', "Invalid King choice");
            TestPromotionChoice(board, method, "P", false, '\0', "Invalid Pawn choice");
            TestPromotionChoice(board, method, "X", false, '\0', "Invalid random character");
            TestPromotionChoice(board, method, "1", false, '\0', "Invalid number");
            TestPromotionChoice(board, method, "@", false, '\0', "Invalid symbol");
            
            // Test edge cases
            TestPromotionChoice(board, method, "", false, '\0', "Empty string");
            TestPromotionChoice(board, method, null, false, '\0', "Null input");
            TestPromotionChoice(board, method, " ", false, '\0', "Whitespace only");
            TestPromotionChoice(board, method, "  Q  ", true, 'Q', "Queen with surrounding whitespace");
            
            Console.WriteLine("Piece selection validation tests completed.");
            Console.WriteLine();
            
            // Run piece generation tests
            RunPieceGenerationTests();
        }
        
        private static void RunPieceGenerationTests()
        {
            Console.WriteLine("Running Promoted Piece Generation Tests...");
            
            var board = new ChessBoard();
            
            // Test CreatePromotedPiece method using reflection
            var method = typeof(ChessBoard).GetMethod("CreatePromotedPiece", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test valid white piece generation
            TestPieceGeneration(board, method, "W", 'Q', "WQ", "White Queen generation");
            TestPieceGeneration(board, method, "W", 'R', "WR", "White Rook generation");
            TestPieceGeneration(board, method, "W", 'B', "WB", "White Bishop generation");
            TestPieceGeneration(board, method, "W", 'N', "WN", "White Knight generation");
            
            // Test valid black piece generation
            TestPieceGeneration(board, method, "B", 'Q', "BQ", "Black Queen generation");
            TestPieceGeneration(board, method, "B", 'R', "BR", "Black Rook generation");
            TestPieceGeneration(board, method, "B", 'B', "BB", "Black Bishop generation");
            TestPieceGeneration(board, method, "B", 'N', "BN", "Black Knight generation");
            
            // Test case insensitive piece type handling
            TestPieceGeneration(board, method, "W", 'q', "WQ", "White Queen generation (lowercase input)");
            TestPieceGeneration(board, method, "B", 'r', "BR", "Black Rook generation (lowercase input)");
            TestPieceGeneration(board, method, "W", 'b', "WB", "White Bishop generation (lowercase input)");
            TestPieceGeneration(board, method, "B", 'n', "BN", "Black Knight generation (lowercase input)");
            
            // Test invalid color inputs
            TestPieceGenerationException(board, method, "X", 'Q', "Invalid color 'X'");
            TestPieceGenerationException(board, method, "w", 'Q', "Invalid color 'w' (lowercase)");
            TestPieceGenerationException(board, method, "b", 'Q', "Invalid color 'b' (lowercase)");
            TestPieceGenerationException(board, method, "", 'Q', "Empty color string");
            TestPieceGenerationException(board, method, "WB", 'Q', "Multi-character color");
            
            // Test invalid piece type inputs
            TestPieceGenerationException(board, method, "W", 'K', "Invalid piece type 'K' (King)");
            TestPieceGenerationException(board, method, "W", 'P', "Invalid piece type 'P' (Pawn)");
            TestPieceGenerationException(board, method, "W", 'X', "Invalid piece type 'X'");
            TestPieceGenerationException(board, method, "W", '1', "Invalid piece type '1' (number)");
            TestPieceGenerationException(board, method, "W", '@', "Invalid piece type '@' (symbol)");
            
            Console.WriteLine("Promoted piece generation tests completed.");
        }
        
        private static void TestPieceGeneration(ChessBoard board, MethodInfo method, string color, char pieceType, string expectedResult, string testName)
        {
            try
            {
                string result = (string)method.Invoke(board, new object[] { color, pieceType });
                bool testPassed = (result == expectedResult);
                string status = testPassed ? "PASS" : "FAIL";
                Console.WriteLine($"Test - {testName}: {status} (Expected: {expectedResult}, Got: {result})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test - {testName}: FAIL (Unexpected exception: {ex.Message})");
            }
        }
        
        private static void TestPieceGenerationException(ChessBoard board, MethodInfo method, string color, char pieceType, string testName)
        {
            try
            {
                string result = (string)method.Invoke(board, new object[] { color, pieceType });
                Console.WriteLine($"Test - {testName}: FAIL (Expected exception but got result: {result})");
            }
            catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException)
            {
                Console.WriteLine($"Test - {testName}: PASS (Correctly threw ArgumentException: {ex.InnerException.Message})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test - {testName}: FAIL (Wrong exception type: {ex.GetType().Name} - {ex.Message})");
            }
        }
        
        private static void TestPromotionChoice(ChessBoard board, MethodInfo method, string input, bool expectedValid, char expectedChoice, string testName)
        {
            // Prepare parameters for the method call
            object[] parameters = new object[] { input, null };
            
            try
            {
                // Call the method
                bool result = (bool)method.Invoke(board, parameters);
                char actualChoice = (char)parameters[1]; // out parameter
                
                // Check if the result matches expectations
                bool testPassed = (result == expectedValid);
                if (expectedValid && result)
                {
                    testPassed = testPassed && (actualChoice == expectedChoice);
                }
                
                string status = testPassed ? "PASS" : "FAIL";
                string details = expectedValid 
                    ? $"Expected: Valid={expectedValid}, Choice={expectedChoice}, Got: Valid={result}, Choice={actualChoice}"
                    : $"Expected: Valid={expectedValid}, Got: Valid={result}";
                
                Console.WriteLine($"Test - {testName}: {status} ({details})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test - {testName}: FAIL (Exception: {ex.Message})");
            }
        }
    }
}