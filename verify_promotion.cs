using System;
using System.Reflection;

namespace ChessEngine
{
    class VerifyPromotion
    {
        static void Main()
        {
            Console.WriteLine("=== Testing HandlePawnPromotion Method ===");
            
            ChessBoard board = new ChessBoard();
            
            // Use reflection to access private methods and fields
            MethodInfo handlePromotionMethod = typeof(ChessBoard).GetMethod("HandlePawnPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            FieldInfo boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            if (handlePromotionMethod == null)
            {
                Console.WriteLine("ERROR: HandlePawnPromotion method not found!");
                return;
            }
            
            Console.WriteLine("✓ HandlePawnPromotion method found");
            
            // Test 1: AI Promotion
            Console.WriteLine("\nTest 1: AI Promotion to Queen");
            boardArray[0, 4] = "."; // Clear target square
            
            try 
            {
                handlePromotionMethod.Invoke(board, new object[] { 0, 4, "W", true });
                string result = boardArray[0, 4];
                
                if (result == "WQ")
                {
                    Console.WriteLine("✓ AI promotion successful: " + result);
                }
                else
                {
                    Console.WriteLine("✗ AI promotion failed. Expected 'WQ', got: " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("✗ AI promotion threw exception: " + ex.Message);
            }
            
            // Test 2: Black AI Promotion
            Console.WriteLine("\nTest 2: Black AI Promotion to Queen");
            boardArray[7, 3] = "."; // Clear target square
            
            try 
            {
                handlePromotionMethod.Invoke(board, new object[] { 7, 3, "B", true });
                string result = boardArray[7, 3];
                
                if (result == "BQ")
                {
                    Console.WriteLine("✓ Black AI promotion successful: " + result);
                }
                else
                {
                    Console.WriteLine("✗ Black AI promotion failed. Expected 'BQ', got: " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("✗ Black AI promotion threw exception: " + ex.Message);
            }
            
            // Test 3: Verify CreatePromotedPiece method
            Console.WriteLine("\nTest 3: CreatePromotedPiece method");
            MethodInfo createPromotedPieceMethod = typeof(ChessBoard).GetMethod("CreatePromotedPiece", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (createPromotedPieceMethod != null)
            {
                try
                {
                    string wq = (string)createPromotedPieceMethod.Invoke(board, new object[] { "W", 'Q' });
                    string br = (string)createPromotedPieceMethod.Invoke(board, new object[] { "B", 'R' });
                    string wb = (string)createPromotedPieceMethod.Invoke(board, new object[] { "W", 'B' });
                    string bn = (string)createPromotedPieceMethod.Invoke(board, new object[] { "B", 'N' });
                    
                    Console.WriteLine($"✓ CreatePromotedPiece results: {wq}, {br}, {wb}, {bn}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("✗ CreatePromotedPiece threw exception: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("✗ CreatePromotedPiece method not found");
            }
            
            Console.WriteLine("\n=== Verification Complete ===");
        }
    }
}