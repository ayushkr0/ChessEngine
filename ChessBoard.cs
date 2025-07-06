using System;
using System.Collections.Generic;

namespace ChessEngine
{
    public class ChessBoard
    {
        private readonly string[,] board;
        private string currentTurn = "W"; // White starts
        private bool whiteKingMoved = false, blackKingMoved = false;
        private bool whiteLeftRookMoved = false, whiteRightRookMoved = false;
        private bool blackLeftRookMoved = false, blackRightRookMoved = false;

        public ChessBoard()
        {
            board = new string[8, 8];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            string[] backRank = { "R", "N", "B", "Q", "K", "B", "N", "R" };

            for (int i = 0; i < 8; i++)
            {
                board[0, i] = "B" + backRank[i];
                board[1, i] = "BP";
                board[6, i] = "WP";
                board[7, i] = "W" + backRank[i];
            }

            for (int row = 2; row <= 5; row++)
                for (int col = 0; col < 8; col++)
                    board[row, col] = ".";
        }

        public int EvaluateBoard()
        {
            int score = 0;
            var pieceValues = new Dictionary<string, int>
            {
                ["P"] = 100,
                ["N"] = 320,
                ["B"] = 330,
                ["R"] = 500,
                ["Q"] = 900,
                ["K"] = 20000
            };

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string piece = board[row, col];
                    if (piece == ".") continue;

                    int value = pieceValues[piece[1].ToString()];
                    score += (piece[0] == 'W') ? value : -value;
                }
            }

            return score;
        }

        public void MakeBestAIMove(int depth)
        {
            int bestScore = int.MinValue;
            var bestMoves = new List<(int fromRow, int fromCol, int toRow, int toCol)>();

            for (int fromRow = 0; fromRow < 8; fromRow++)
            {
                for (int fromCol = 0; fromCol < 8; fromCol++)
                {
                    string piece = board[fromRow, fromCol];
                    if (piece == "." || piece[0].ToString() != currentTurn) continue;

                    for (int toRow = 0; toRow < 8; toRow++)
                    {
                        for (int toCol = 0; toCol < 8; toCol++)
                        {
                            if (fromRow == toRow && fromCol == toCol) continue;

                            string backupFrom = board[fromRow, fromCol];
                            string backupTo = board[toRow, toCol];

                            string type = piece[1].ToString();
                            bool valid = type switch
                            {
                                "P" => IsValidPawnMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                "R" => IsValidRookMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                "N" => IsValidKnightMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                "B" => IsValidBishopMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                "Q" => IsValidQueenMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                "K" => IsValidKingMove(fromRow, fromCol, toRow, toCol, currentTurn),
                                _ => false
                            };

                            if (!valid) continue;

                            board[toRow, toCol] = board[fromRow, fromCol];
                            board[fromRow, fromCol] = ".";

                            if (IsKingInCheck(currentTurn))
                            {
                                board[fromRow, fromCol] = backupFrom;
                                board[toRow, toCol] = backupTo;
                                continue;
                            }

                            int score = Minimax(depth - 1, false, int.MinValue, int.MaxValue);
                            board[fromRow, fromCol] = backupFrom;
                            board[toRow, toCol] = backupTo;

                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestMoves.Clear();
                                bestMoves.Add((fromRow, fromCol, toRow, toCol));
                            }
                            else if (score == bestScore)
                            {
                                bestMoves.Add((fromRow, fromCol, toRow, toCol));
                            }
                        }
                    }
                }
            }

            if (bestMoves.Count > 0)
            {
                Random rand = new Random();
                var chosen = bestMoves[rand.Next(bestMoves.Count)];
                MovePiece(chosen.fromRow, chosen.fromCol, chosen.toRow, chosen.toCol);
            }
        }
        public string CurrentTurn()
        {
            return currentTurn;
        }

        public (int, int) ParsePosition(string pos)
        {
            if (pos.Length != 2) return (-1, -1);
            int col = pos[0] - 'a';
            int row = 8 - (pos[1] - '0');
            if (col < 0 || col > 7 || row < 0 || row > 7) return (-1, -1);
            return (row, col);
        }

        public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            string piece = board[fromRow, fromCol];
            if (piece == "." || piece == null)
            {
                Console.WriteLine("No piece at the selected position.");
                return;
            }

            string color = piece[0].ToString();
            string type = piece[1].ToString();

            if (color != currentTurn)
            {
                Console.WriteLine($"It's not your turn! It's {(currentTurn == "W" ? "White" : "Black")}'s turn.");
                return;
            }

            bool validMove = type switch
            {
                "P" => IsValidPawnMove(fromRow, fromCol, toRow, toCol, color),
                "R" => IsValidRookMove(fromRow, fromCol, toRow, toCol, color),
                "N" => IsValidKnightMove(fromRow, fromCol, toRow, toCol, color),
                "B" => IsValidBishopMove(fromRow, fromCol, toRow, toCol, color),
                "Q" => IsValidQueenMove(fromRow, fromCol, toRow, toCol, color),
                "K" => IsValidKingMove(fromRow, fromCol, toRow, toCol, color),
                _ => false
            };

            if (!validMove)
            {
                Console.WriteLine("Invalid move for piece: " + piece);
                return;
            }

            string captured = board[toRow, toCol];
            board[toRow, toCol] = piece;
            board[fromRow, fromCol] = ".";

            bool kingInCheck = IsKingInCheck(color);
            if (kingInCheck)
            {
                board[fromRow, fromCol] = piece;
                board[toRow, toCol] = captured;
                Console.WriteLine("You cannot move into check.");
                return;
            }

            if (type == "K")
            {
                if (color == "W") whiteKingMoved = true;
                else blackKingMoved = true;
            }

            if (type == "R")
            {
                if (color == "W" && fromRow == 7)
                {
                    if (fromCol == 0) whiteLeftRookMoved = true;
                    if (fromCol == 7) whiteRightRookMoved = true;
                }
                else if (color == "B" && fromRow == 0)
                {
                    if (fromCol == 0) blackLeftRookMoved = true;
                    if (fromCol == 7) blackRightRookMoved = true;
                }
            }

            currentTurn = currentTurn == "W" ? "B" : "W";

            if (IsCheckmate(currentTurn))
            {
                Console.WriteLine($"Checkmate! {color} wins!");
            }
        }

        // ------------ VALID MOVE CHECKS BELOW ------------

        private bool IsValidPawnMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int dir = (color == "W") ? -1 : 1;
            int startRow = (color == "W") ? 6 : 1;

            if (fromCol == toCol && board[toRow, toCol] == ".")
            {
                if (toRow - fromRow == dir) return true;
                if (fromRow == startRow && toRow - fromRow == 2 * dir && board[fromRow + dir, toCol] == ".") return true;
            }

            if (Math.Abs(fromCol - toCol) == 1 && toRow - fromRow == dir &&
                board[toRow, toCol] != "." && board[toRow, toCol][0].ToString() != color)
                return true;

            return false;
        }

        private bool IsValidRookMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            if (fromRow != toRow && fromCol != toCol) return false;

            int rowDir = Math.Sign(toRow - fromRow);
            int colDir = Math.Sign(toCol - fromCol);
            int r = fromRow + rowDir, c = fromCol + colDir;

            while (r != toRow || c != toCol)
            {
                if (board[r, c] != ".") return false;
                r += rowDir;
                c += colDir;
            }

            return board[toRow, toCol] == "." || board[toRow, toCol][0].ToString() != color;
        }

        private bool IsValidKnightMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int dr = Math.Abs(fromRow - toRow), dc = Math.Abs(fromCol - toCol);
            if (!((dr == 2 && dc == 1) || (dr == 1 && dc == 2))) return false;
            return board[toRow, toCol] == "." || board[toRow, toCol][0].ToString() != color;
        }

        private bool IsValidBishopMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol)) return false;

            int rowDir = Math.Sign(toRow - fromRow), colDir = Math.Sign(toCol - fromCol);
            int r = fromRow + rowDir, c = fromCol + colDir;

            while (r != toRow && c != toCol)
            {
                if (board[r, c] != ".") return false;
                r += rowDir;
                c += colDir;
            }

            return board[toRow, toCol] == "." || board[toRow, toCol][0].ToString() != color;
        }

        private bool IsValidQueenMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            return IsValidBishopMove(fromRow, fromCol, toRow, toCol, color) ||
                   IsValidRookMove(fromRow, fromCol, toRow, toCol, color);
        }

        private bool IsValidKingMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int dr = Math.Abs(fromRow - toRow), dc = Math.Abs(fromCol - toCol);
            if (dr <= 1 && dc <= 1)
                return board[toRow, toCol] == "." || board[toRow, toCol][0].ToString() != color;

            // Castling
            if (color == "W" && fromRow == 7 && fromCol == 4)
            {
                if (toRow == 7 && toCol == 6 && !whiteKingMoved && !whiteRightRookMoved &&
                    board[7, 5] == "." && board[7, 6] == "." &&
                    !IsKingInCheck("W") && !SquareUnderAttack(7, 5, "B") && !SquareUnderAttack(7, 6, "B"))
                {
                    board[7, 5] = "WR";
                    board[7, 7] = ".";
                    return true;
                }

                if (toRow == 7 && toCol == 2 && !whiteKingMoved && !whiteLeftRookMoved &&
                    board[7, 1] == "." && board[7, 2] == "." && board[7, 3] == "." &&
                    !IsKingInCheck("W") && !SquareUnderAttack(7, 3, "B") && !SquareUnderAttack(7, 2, "B"))
                {
                    board[7, 3] = "WR";
                    board[7, 0] = ".";
                    return true;
                }
            }

            if (color == "B" && fromRow == 0 && fromCol == 4)
            {
                if (toRow == 0 && toCol == 6 && !blackKingMoved && !blackRightRookMoved &&
                    board[0, 5] == "." && board[0, 6] == "." &&
                    !IsKingInCheck("B") && !SquareUnderAttack(0, 5, "W") && !SquareUnderAttack(0, 6, "W"))
                {
                    board[0, 5] = "BR";
                    board[0, 7] = ".";
                    return true;
                }

                if (toRow == 0 && toCol == 2 && !blackKingMoved && !blackLeftRookMoved &&
                    board[0, 1] == "." && board[0, 2] == "." && board[0, 3] == "." &&
                    !IsKingInCheck("B") && !SquareUnderAttack(0, 3, "W") && !SquareUnderAttack(0, 2, "W"))
                {
                    board[0, 3] = "BR";
                    board[0, 0] = ".";
                    return true;
                }
            }

            return false;
        }

        public bool IsKingInCheck(string color)
        {
            (int kingRow, int kingCol) = FindKing(color);
            return SquareUnderAttack(kingRow, kingCol, color == "W" ? "B" : "W");
        }

        private (int, int) FindKing(string color)
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (board[r, c] == color + "K")
                        return (r, c);
            return (-1, -1);
        }

        private bool SquareUnderAttack(int row, int col, string byColor)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    string piece = board[r, c];
                    if (piece == "." || piece[0].ToString() != byColor) continue;

                    string type = piece[1].ToString();
                    bool attacks = type switch
                    {
                        "P" => IsValidPawnMove(r, c, row, col, byColor),
                        "R" => IsValidRookMove(r, c, row, col, byColor),
                        "N" => IsValidKnightMove(r, c, row, col, byColor),
                        "B" => IsValidBishopMove(r, c, row, col, byColor),
                        "Q" => IsValidQueenMove(r, c, row, col, byColor),
                        "K" => Math.Max(Math.Abs(row - r), Math.Abs(col - c)) == 1,
                        _ => false
                    };

                    if (attacks) return true;
                }
            }

            return false;
        }

        public bool IsCheckmate(string color)
        {
            if (!IsKingInCheck(color)) return false;

            for (int fromRow = 0; fromRow < 8; fromRow++)
            {
                for (int fromCol = 0; fromCol < 8; fromCol++)
                {
                    string piece = board[fromRow, fromCol];
                    if (piece == "." || piece[0].ToString() != color)
                        continue;

                    for (int toRow = 0; toRow < 8; toRow++)
                    {
                        for (int toCol = 0; toCol < 8; toCol++)
                        {
                            if (fromRow == toRow && fromCol == toCol) continue;

                            string backupFrom = board[fromRow, fromCol];
                            string backupTo = board[toRow, toCol];

                            string type = piece[1].ToString();
                            bool valid = type switch
                            {
                                "P" => IsValidPawnMove(fromRow, fromCol, toRow, toCol, color),
                                "R" => IsValidRookMove(fromRow, fromCol, toRow, toCol, color),
                                "N" => IsValidKnightMove(fromRow, fromCol, toRow, toCol, color),
                                "B" => IsValidBishopMove(fromRow, fromCol, toRow, toCol, color),
                                "Q" => IsValidQueenMove(fromRow, fromCol, toRow, toCol, color),
                                "K" => IsValidKingMove(fromRow, fromCol, toRow, toCol, color),
                                _ => false
                            };

                            if (!valid) continue;

                            board[toRow, toCol] = board[fromRow, fromCol];
                            board[fromRow, fromCol] = ".";

                            bool stillInCheck = IsKingInCheck(color);

                            board[fromRow, fromCol] = backupFrom;
                            board[toRow, toCol] = backupTo;

                            if (!stillInCheck) return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
