using System;

namespace ChessEngine
{
    public class ChessGame
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Chess Game...");
            ChessBoard board = new ChessBoard();
            board.PrintBoard();
            string currentTurn = board.CurrentTurn();
            
            while (true)
            {
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
                        board.PrintBoard();
                        currentTurn = board.CurrentTurn();
                        continue;
                    }
                    else if (command == "redo")
                    {
                        board.RedoMove();
                        board.PrintBoard();
                        currentTurn = board.CurrentTurn();
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
                        Console.WriteLine("• exit: Quit the game");
                        Console.WriteLine("========================\n");
                        continue;
                    }
                    else if (command == "exit")
                    {
                        Console.WriteLine("Thanks for playing!");
                        break;
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
                    
                    try
                    {
                        board.MovePiece(fromRow, fromCol, toRow, toCol);
                        board.PrintBoard();
                        currentTurn = board.CurrentTurn();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else // AI (Black)
                {
                    Console.WriteLine("AI is thinking...");
                    System.Threading.Thread.Sleep(1000); // 1 second delay for realism
                    board.MakeBestAIMove(3); // Depth 3 is decent; change if needed
                    board.PrintBoard();
                    currentTurn = board.CurrentTurn();
                }
            }
        }
    }
}