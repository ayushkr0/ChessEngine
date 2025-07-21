using System;

namespace ChessEngine
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("PROGRAM STARTED");
                Console.Out.Flush();
                Console.WriteLine($"Args count: {args.Length}");
                Console.Out.Flush();
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"Arg[{i}]: '{args[i]}'");
                    Console.Out.Flush();
                }
                
                // Force test for debugging
                if (args.Length == 0)
                {
                    Console.WriteLine("No args provided, running existing tests...");
                    Console.Out.Flush();
                    PromotionTest.RunTests();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION IN MAIN: {ex.Message}");
                Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
                Console.Out.Flush();
                throw;
            }
            
            // Check if we should run tests
            if (args.Length > 0 && args[0] == "test")
            {
                Console.WriteLine("Running tests...");
                PromotionTest.RunTests();
                
                // Run HandlePawnPromotion tests
                TestHandlePawnPromotion handlePromotionTests = new TestHandlePawnPromotion();
                handlePromotionTests.RunAllTests();
                
                // Run integration tests
                TestPromotionIntegrationSimple.RunTests();
                return;
            }
            
            // Check if we should run AI promotion tests
            if (args.Length > 0 && args[0] == "ai-test")
            {
                Console.WriteLine("Running AI Promotion tests...");
                TestAIPromotion.RunAllTests();
                return;
            }
            
            // Check if we should verify AI promotion
            if (args.Length > 0 && args[0] == "verify-ai")
            {
                Console.WriteLine("Running AI Promotion verification...");
                // VerifyAIPromotion.Main(args); // Commented out due to access level
                return;
            }
            
            // Check if we should run simple AI promotion test
            if (args.Length > 0 && args[0] == "simple-ai-test")
            {
                Console.WriteLine("Running Simple AI Promotion test...");
                // SimpleAIPromotionTest.Main(args); // Commented out due to access level
                return;
            }
            
            // Check if we should run multiple queens test
            if (args.Length > 0 && args[0] == "multiple-queens-test")
            {
                Console.WriteLine("Running Multiple Queens Support tests...");
                TestMultipleQueensSupport.RunAllTests();
                return;
            }
            
            // Check if we should run comprehensive integration tests
            if (args.Length > 0 && args[0] == "comprehensive-test")
            {
                Console.WriteLine("Running Comprehensive Promotion Integration tests...");
                TestComprehensivePromotionIntegration.RunAllTests();
                return;
            }
            
            // Check if we should run en passant state management tests
            if (args.Length > 0 && args[0] == "en-passant-state-test")
            {
                Console.WriteLine("Running En Passant State Management tests...");
                TestEnPassantStateManagement.RunAllTests();
                return;
            }
            
            // Check if we should run en passant error handling tests
            if (args.Length > 0 && args[0] == "en-passant-error-test")
            {
                Console.WriteLine("Running En Passant Error Handling tests...");
                TestEnPassantErrorHandling.RunAllTests();
                return;
            }
            
            ChessBoard board = new ChessBoard();

            while (true)
            {
                board.PrintBoard();
                string currentTurn = board.CurrentTurn();

                if (currentTurn == "W") // Player (White)
                {
                    Console.Write("Enter your move (e.g., e2 e4) or command (undo, redo, history, help): ");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("No input received. Try again.");
                        continue;
                    }

                    string command = input.Trim().ToLower();

                    // Handle special commands
                    if (command == "undo")
                    {
                        board.UndoMove();
                        continue;
                    }
                    else if (command == "redo")
                    {
                        board.RedoMove();
                        continue;
                    }
                    else if (command == "history")
                    {
                        board.PrintMoveHistory();
                        continue;
                    }
                    else if (command == "help")
                    {
                        Console.WriteLine("\n=== Available Commands ===");
                        Console.WriteLine("• Move: e2 e4 (from square to square)");
                        Console.WriteLine("• undo: Undo the last move");
                        Console.WriteLine("• redo: Redo the last undone move");
                        Console.WriteLine("• history: Show move history");
                        Console.WriteLine("• help: Show this help message");
                        Console.WriteLine("========================\n");
                        continue;
                    }

                    // Handle regular moves
                    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Invalid input. Please use format like 'e2 e4' or type 'help' for commands.");
                        continue;
                    }

                    var (fromRow, fromCol) = board.ParsePosition(parts[0]);
                    var (toRow, toCol) = board.ParsePosition(parts[1]);

                    if (fromRow == -1 || toRow == -1)
                    {
                        Console.WriteLine("Invalid coordinates. Use valid chess positions like 'e2'.");
                        continue;
                    }

                    board.MovePiece(fromRow, fromCol, toRow, toCol);
                }
                else // AI (Black)
                {
                    Console.WriteLine("AI is thinking...");
                    System.Threading.Thread.Sleep(1000); // 1 second delay for realism
                    board.MakeBestAIMove(3); // Depth 3 is decent; change if needed
                }

                Console.WriteLine();
            }
        }
    }
}
