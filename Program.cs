using System;

namespace ChessEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            ChessBoard board = new ChessBoard();

            while (true)
            {
                board.PrintBoard();
                string currentTurn = board.CurrentTurn();

                if (currentTurn == "W") // Player (White)
                {
                    Console.Write("Enter your move (e.g., e2 e4): ");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("No input received. Try again.");
                        continue;
                    }

                    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Invalid input. Please use format like 'e2 e4'.");
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
                    board.MakeBestAIMove(3); // Depth 3 is decent; change if needed
                }

                Console.WriteLine();
            }
        }
    }
}
