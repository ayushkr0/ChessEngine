using System;

namespace ChessEngine
{
    public class ChessRunner
    {
        public void Run()
        {
            ChessBoard board = new ChessBoard();
            board.PrintBoard();

            while (true)
            {
                Console.Write("\nEnter your move (e.g., e2 e4): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid format. Use e.g., e2 e4");
                    continue;
                }

                var (fromRow, fromCol) = board.ParsePosition(parts[0]);
                var (toRow, toCol) = board.ParsePosition(parts[1]);

                if (fromRow == -1 || toRow == -1)
                {
                    Console.WriteLine("Invalid square notation.");
                    continue;
                }

                board.MovePiece(fromRow, fromCol, toRow, toCol);
                board.PrintBoard();
            }
        }
    }
}
