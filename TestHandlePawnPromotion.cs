using System;
using System.IO;
using System.Reflection;

namespace ChessEngine
{
    public class TestHandlePawnPromotion
    {
        private ChessBoard board;
        
        public void RunAllTests()
        {
            Console.WriteLine("Running HandlePawnPromotion tests...");
            
            TestAIPromotionToQueen();
            TestHumanPromotionWithValidChoice();
            TestPromotionPlacesPieceCorrectly();
            TestPromotionWithAllPieceTypes();
            TestPromotionDisplaysCorrectMessage();
            
            Console.WriteLine("All HandlePawnPromotion tests completed!");
        }
        
        private void TestAIPromotionToQueen()
        {
            Console.WriteLine("Testing AI promotion to Queen...");
            
            board = new ChessBoard();
            
            // Use reflection to access private method
            MethodInfo handlePromotionMethod = typeof(ChessBoard).GetMethod("HandlePawnPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Use reflection to access board array
            FieldInfo boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Clear the target square
            boardArray[0, 4] = ".";
            
            // Call HandlePawnPromotion for AI (should promote to Queen)
            handlePromotionMethod.Invoke(board, new object[] { 0, 4, "W", true });
            
            // Verify the piece was promoted to Queen
            string promotedPiece = boardArray[0, 4];
            if (promotedPiece == "WQ")
            {
                Console.WriteLine("✓ AI promotion to Queen successful");
            }
            else
            {
                Console.WriteLine($"✗ AI promotion failed. Expected 'WQ', got '{promotedPiece}'");
            }
        }
        
        private void TestHumanPromotionWithValidChoice()
        {
            Console.WriteLine("Testing human promotion with simulated input...");
            
            board = new ChessBoard();
            
            // Use reflection to access private methods
            MethodInfo handlePromotionMethod = typeof(ChessBoard).GetMethod("HandlePawnPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo createPromotedPieceMethod = typeof(ChessBoard).GetMethod("CreatePromotedPiece", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Use reflection to access board array
            FieldInfo boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Test each piece type by directly calling CreatePromotedPiece
            char[] pieceTypes = { 'Q', 'R', 'B', 'N' };
            string[] expectedPieces = { "WQ", "WR", "WB", "WN" };
            
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                string result = (string)createPromotedPieceMethod.Invoke(board, new object[] { "W", pieceTypes[i] });
                if (result == expectedPieces[i])
                {
                    Console.WriteLine($"✓ CreatePromotedPiece works for {pieceTypes[i]} -> {expectedPieces[i]}");
                }
                else
                {
                    Console.WriteLine($"✗ CreatePromotedPiece failed for {pieceTypes[i]}. Expected '{expectedPieces[i]}', got '{result}'");
                }
            }
        }
        
        private void TestPromotionPlacesPieceCorrectly()
        {
            Console.WriteLine("Testing promotion places piece on correct square...");
            
            board = new ChessBoard();
            
            // Use reflection to access private method
            MethodInfo handlePromotionMethod = typeof(ChessBoard).GetMethod("HandlePawnPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Use reflection to access board array
            FieldInfo boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Test different squares
            int[] testRows = { 0, 7 };
            int[] testCols = { 0, 3, 7 };
            string[] testColors = { "W", "B" };
            
            foreach (int row in testRows)
            {
                foreach (int col in testCols)
                {
                    foreach (string color in testColors)
                    {
                        // Clear the square
                        boardArray[row, col] = ".";
                        
                        // Promote AI pawn (always Queen)
                        handlePromotionMethod.Invoke(board, new object[] { row, col, color, true });
                        
                        // Check if piece is placed correctly
                        string expectedPiece = color + "Q";
                        string actualPiece = boardArray[row, col];
                        
                        if (actualPiece == expectedPiece)
                        {
                            Console.WriteLine($"✓ Piece placed correctly at [{row},{col}]: {expectedPiece}");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Piece placement failed at [{row},{col}]. Expected '{expectedPiece}', got '{actualPiece}'");
                        }
                    }
                }
            }
        }
        
        private void TestPromotionWithAllPieceTypes()
        {
            Console.WriteLine("Testing promotion with all piece types...");
            
            board = new ChessBoard();
            
            // Use reflection to access private method
            MethodInfo createPromotedPieceMethod = typeof(ChessBoard).GetMethod("CreatePromotedPiece", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Test all combinations of colors and piece types
            string[] colors = { "W", "B" };
            char[] pieceTypes = { 'Q', 'R', 'B', 'N' };
            string[,] expectedResults = {
                { "WQ", "WR", "WB", "WN" },
                { "BQ", "BR", "BB", "BN" }
            };
            
            for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
            {
                for (int pieceIndex = 0; pieceIndex < pieceTypes.Length; pieceIndex++)
                {
                    string result = (string)createPromotedPieceMethod.Invoke(board, 
                        new object[] { colors[colorIndex], pieceTypes[pieceIndex] });
                    string expected = expectedResults[colorIndex, pieceIndex];
                    
                    if (result == expected)
                    {
                        Console.WriteLine($"✓ {colors[colorIndex]} + {pieceTypes[pieceIndex]} = {expected}");
                    }
                    else
                    {
                        Console.WriteLine($"✗ {colors[colorIndex]} + {pieceTypes[pieceIndex]} failed. Expected '{expected}', got '{result}'");
                    }
                }
            }
        }
        
        private void TestPromotionDisplaysCorrectMessage()
        {
            Console.WriteLine("Testing promotion displays correct messages...");
            
            board = new ChessBoard();
            
            // Use reflection to access private method
            MethodInfo handlePromotionMethod = typeof(ChessBoard).GetMethod("HandlePawnPromotion", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Use reflection to access board array
            FieldInfo boardField = typeof(ChessBoard).GetField("board", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            string[,] boardArray = (string[,])boardField.GetValue(board);
            
            // Capture console output
            StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Clear the target square
            boardArray[0, 4] = ".";
            
            // Test AI promotion message
            handlePromotionMethod.Invoke(board, new object[] { 0, 4, "W", true });
            
            // Reset console output
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            
            string output = stringWriter.ToString();
            if (output.Contains("AI promoted pawn to Queen!"))
            {
                Console.WriteLine("✓ AI promotion message displayed correctly");
            }
            else
            {
                Console.WriteLine($"✗ AI promotion message incorrect. Output: {output.Trim()}");
            }
            
            // Test that the piece was actually placed
            if (boardArray[0, 4] == "WQ")
            {
                Console.WriteLine("✓ AI promotion placed Queen correctly");
            }
            else
            {
                Console.WriteLine($"✗ AI promotion piece placement failed. Expected 'WQ', got '{boardArray[0, 4]}'");
            }
        }
    }
}