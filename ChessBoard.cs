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
        
        // En passant state tracking
        private (int row, int col) enPassantTarget = (-1, -1);  // Square where en passant capture can occur
        private (int row, int col) enPassantVictim = (-1, -1);  // Pawn that can be captured via en passant

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
            
            // Enhanced piece values
            var pieceValues = new Dictionary<string, int>
            {
                ["P"] = 100,
                ["N"] = 320,
                ["B"] = 330,
                ["R"] = 500,
                ["Q"] = 900,
                ["K"] = 20000
            };

            // Material evaluation
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string piece = board[row, col];
                    if (piece == ".") continue;

                    string color = piece[0].ToString();
                    string type = piece[1].ToString();
                    int value = pieceValues[type];
                    
                    // Add positional bonuses
                    value += GetPositionalBonus(type, row, col, color);
                    
                    score += (color == "W") ? value : -value;
                }
            }
            
            // Add strategic bonuses
            score += EvaluateKingSafety("W") - EvaluateKingSafety("B");
            score += EvaluateCenterControl("W") - EvaluateCenterControl("B");
            score += EvaluatePawnStructure("W") - EvaluatePawnStructure("B");
            
            return score;
        }
        
        /// <summary>
        /// Gets positional bonus for a piece based on its position
        /// </summary>
        private int GetPositionalBonus(string pieceType, int row, int col, string color)
        {
            switch (pieceType)
            {
                case "P":
                    return GetPawnPositionalBonus(row, col, color);
                case "N":
                    return GetKnightPositionalBonus(row, col);
                case "B":
                    return GetBishopPositionalBonus(row, col);
                case "R":
                    return GetRookPositionalBonus(row, col);
                case "Q":
                    return GetQueenPositionalBonus(row, col);
                case "K":
                    return GetKingPositionalBonus(row, col, color);
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Evaluates pawn positional value
        /// </summary>
        private int GetPawnPositionalBonus(int row, int col, string color)
        {
            int bonus = 0;
            
            // Encourage pawn advancement
            if (color == "W")
            {
                bonus += (7 - row) * 5; // White pawns get bonus for advancing
            }
            else
            {
                bonus += row * 5; // Black pawns get bonus for advancing
            }
            
            // Center pawns are more valuable
            if (col >= 2 && col <= 5)
            {
                bonus += 10;
            }
            
            // Extra bonus for central pawns
            if (col == 3 || col == 4)
            {
                bonus += 5;
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Evaluates knight positional value
        /// </summary>
        private int GetKnightPositionalBonus(int row, int col)
        {
            int bonus = 0;
            
            // Knights are better in the center
            double centerDistance = Math.Max(Math.Abs(row - 3.5), Math.Abs(col - 3.5));
            bonus += (int)((4 - centerDistance) * 5);
            
            // Penalty for knights on the rim
            if (row == 0 || row == 7 || col == 0 || col == 7)
            {
                bonus -= 20;
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Evaluates bishop positional value
        /// </summary>
        private int GetBishopPositionalBonus(int row, int col)
        {
            int bonus = 0;
            
            // Bishops prefer central squares
            if ((row >= 2 && row <= 5) && (col >= 2 && col <= 5))
            {
                bonus += 10;
            }
            
            // Long diagonals are valuable
            if ((row + col == 7) || (row - col == 0))
            {
                bonus += 15;
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Evaluates rook positional value
        /// </summary>
        private int GetRookPositionalBonus(int row, int col)
        {
            int bonus = 0;
            
            // Rooks on open files (simplified check)
            bool hasOwnPawn = false;
            for (int r = 0; r < 8; r++)
            {
                if (board[r, col].EndsWith("P"))
                {
                    hasOwnPawn = true;
                    break;
                }
            }
            
            if (!hasOwnPawn)
            {
                bonus += 25; // Open file bonus
            }
            
            // 7th rank bonus for rooks
            if (row == 1 || row == 6)
            {
                bonus += 20;
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Evaluates queen positional value
        /// </summary>
        private int GetQueenPositionalBonus(int row, int col)
        {
            // Queens are flexible, small center preference
            if ((row >= 2 && row <= 5) && (col >= 2 && col <= 5))
            {
                return 5;
            }
            return 0;
        }
        
        /// <summary>
        /// Evaluates king positional value
        /// </summary>
        private int GetKingPositionalBonus(int row, int col, string color)
        {
            int bonus = 0;
            
            // In opening/middlegame, king safety is paramount
            // Encourage castling by giving bonus for corner positions
            if (color == "W")
            {
                if (row == 7 && (col == 1 || col == 2 || col == 6))
                {
                    bonus += 30; // Castled position bonus
                }
            }
            else
            {
                if (row == 0 && (col == 1 || col == 2 || col == 6))
                {
                    bonus += 30; // Castled position bonus
                }
            }
            
            return bonus;
        }
        
        /// <summary>
        /// Evaluates king safety
        /// </summary>
        private int EvaluateKingSafety(string color)
        {
            // Find king position
            (int kingRow, int kingCol) = FindKing(color);
            if (kingRow == -1) return -1000; // King not found (shouldn't happen)
            
            int safety = 0;
            
            // Check for pawn shield
            int direction = (color == "W") ? -1 : 1;
            int shieldRow = kingRow + direction;
            
            if (shieldRow >= 0 && shieldRow < 8)
            {
                for (int col = Math.Max(0, kingCol - 1); col <= Math.Min(7, kingCol + 1); col++)
                {
                    if (board[shieldRow, col] == color + "P")
                    {
                        safety += 10; // Pawn shield bonus
                    }
                }
            }
            
            return safety;
        }
        
        /// <summary>
        /// Evaluates center control
        /// </summary>
        private int EvaluateCenterControl(string color)
        {
            int control = 0;
            
            // Check control of central squares (d4, d5, e4, e5)
            int[] centerRows = { 3, 4 };
            int[] centerCols = { 3, 4 };
            
            foreach (int row in centerRows)
            {
                foreach (int col in centerCols)
                {
                    if (board[row, col] == color + "P")
                    {
                        control += 20; // Pawn in center
                    }
                    else if (IsSquareAttackedBy(row, col, color))
                    {
                        control += 5; // Square attacked by our pieces
                    }
                }
            }
            
            return control;
        }
        
        /// <summary>
        /// Evaluates pawn structure
        /// </summary>
        private int EvaluatePawnStructure(string color)
        {
            int structure = 0;
            
            for (int col = 0; col < 8; col++)
            {
                int pawnCount = 0;
                bool hasPassedPawn = true;
                
                // Count pawns in this file
                for (int row = 0; row < 8; row++)
                {
                    if (board[row, col] == color + "P")
                    {
                        pawnCount++;
                        
                        // Check if this is a passed pawn (simplified)
                        string opponentColor = (color == "W") ? "B" : "W";
                        int direction = (color == "W") ? -1 : 1;
                        
                        for (int checkRow = row + direction; checkRow >= 0 && checkRow < 8; checkRow += direction)
                        {
                            // Check this file and adjacent files for enemy pawns
                            for (int checkCol = Math.Max(0, col - 1); checkCol <= Math.Min(7, col + 1); checkCol++)
                            {
                                if (board[checkRow, checkCol] == opponentColor + "P")
                                {
                                    hasPassedPawn = false;
                                    break;
                                }
                            }
                            if (!hasPassedPawn) break;
                        }
                        
                        if (hasPassedPawn)
                        {
                            structure += 50; // Passed pawn bonus
                        }
                    }
                }
                
                // Penalty for doubled pawns
                if (pawnCount > 1)
                {
                    structure -= (pawnCount - 1) * 15;
                }
            }
            
            return structure;
        }
        
        /// <summary>
        /// Finds the king position for a given color
        /// </summary>
        private (int row, int col) FindKing(string color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == color + "K")
                    {
                        return (row, col);
                    }
                }
            }
            return (-1, -1);
        }
        
        /// <summary>
        /// Checks if a square is attacked by pieces of a given color (simplified)
        /// </summary>
        private bool IsSquareAttackedBy(int targetRow, int targetCol, string color)
        {
            // Simplified attack detection - check if any piece of the given color can move to this square
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string piece = board[row, col];
                    if (piece == "." || piece[0].ToString() != color) continue;
                    
                    string type = piece[1].ToString();
                    bool canAttack = type switch
                    {
                        "P" => CanPawnAttack(row, col, targetRow, targetCol, color),
                        "R" => IsValidRookMove(row, col, targetRow, targetCol, color),
                        "N" => IsValidKnightMove(row, col, targetRow, targetCol, color),
                        "B" => IsValidBishopMove(row, col, targetRow, targetCol, color),
                        "Q" => IsValidQueenMove(row, col, targetRow, targetCol, color),
                        "K" => IsValidKingMove(row, col, targetRow, targetCol, color),
                        _ => false
                    };
                    
                    if (canAttack) return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Checks if a pawn can attack a square (different from normal pawn move)
        /// </summary>
        private bool CanPawnAttack(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int direction = (color == "W") ? -1 : 1;
            return (toRow - fromRow == direction) && (Math.Abs(toCol - fromCol) == 1);
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

                            // Handle en passant capture during AI evaluation
                            bool isEnPassantCapture = false;
                            string enPassantVictimBackup = "";
                            (int victimRow, int victimCol) = (-1, -1);
                            
                            if (type == "P" && backupTo == "." && IsEnPassantCapture(fromRow, fromCol, toRow, toCol, currentTurn))
                            {
                                isEnPassantCapture = true;
                                (victimRow, victimCol) = GetEnPassantVictimPosition();
                                enPassantVictimBackup = board[victimRow, victimCol];
                            }

                            // Execute the move
                            board[toRow, toCol] = board[fromRow, fromCol];
                            board[fromRow, fromCol] = ".";
                            
                            // Execute en passant capture if applicable
                            if (isEnPassantCapture)
                            {
                                board[victimRow, victimCol] = ".";
                            }

                            // Handle pawn promotion during AI evaluation
                            bool promotionOccurred = false;
                            string originalPiece = board[toRow, toCol];
                            if (type == "P" && RequiresPromotion(toRow, currentTurn))
                            {
                                // AI automatically promotes to Queen for evaluation
                                board[toRow, toCol] = CreatePromotedPiece(currentTurn, 'Q');
                                promotionOccurred = true;
                            }

                            if (IsKingInCheck(currentTurn))
                            {
                                // Restore the move
                                board[fromRow, fromCol] = backupFrom;
                                board[toRow, toCol] = backupTo;
                                
                                // Restore en passant victim if applicable
                                if (isEnPassantCapture)
                                {
                                    board[victimRow, victimCol] = enPassantVictimBackup;
                                }
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
                MovePieceAI(chosen.fromRow, chosen.fromCol, chosen.toRow, chosen.toCol);
            }
        }

        // ------------ AI EN PASSANT INTEGRATION ------------

        /// <summary>
        /// Generates all possible en passant moves for the current player
        /// </summary>
        /// <param name="color">Color of the player to generate moves for</param>
        /// <returns>List of en passant moves as tuples (fromRow, fromCol, toRow, toCol)</returns>
        private List<(int fromRow, int fromCol, int toRow, int toCol)> GenerateEnPassantMoves(string color)
        {
            var enPassantMoves = new List<(int fromRow, int fromCol, int toRow, int toCol)>();
            
            // If no en passant opportunity, return empty list
            if (!HasEnPassantOpportunity()) return enPassantMoves;
            
            (int targetRow, int targetCol) = enPassantTarget;
            (int victimRow, int victimCol) = enPassantVictim;
            
            // Find pawns that can execute en passant capture
            int pawnRank = (color == "W") ? 3 : 4; // White pawns on rank 5, Black on rank 4 (0-indexed)
            
            // Check adjacent squares for pawns that can capture
            for (int col = Math.Max(0, victimCol - 1); col <= Math.Min(7, victimCol + 1); col++)
            {
                if (col == victimCol) continue; // Skip the victim's column
                
                string piece = board[pawnRank, col];
                if (piece == color + "P")
                {
                    // Verify this is a valid en passant move
                    if (IsValidEnPassantMove(pawnRank, col, targetRow, targetCol, color))
                    {
                        enPassantMoves.Add((pawnRank, col, targetRow, targetCol));
                    }
                }
            }
            
            return enPassantMoves;
        }

        /// <summary>
        /// Evaluates the tactical value of an en passant capture
        /// </summary>
        /// <param name="fromRow">Starting row of capturing pawn</param>
        /// <param name="fromCol">Starting column of capturing pawn</param>
        /// <param name="toRow">Target row</param>
        /// <param name="toCol">Target column</param>
        /// <param name="color">Color of capturing pawn</param>
        /// <returns>Evaluation score for the en passant capture</returns>
        private int EvaluateEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int baseValue = 100; // Same as regular pawn capture
            int bonusValue = 0;
            
            // Bonus for improving pawn structure
            if (ImprovesePawnStructure(fromRow, fromCol, toRow, toCol, color))
            {
                bonusValue += 25;
            }
            
            // Bonus for creating passed pawn
            if (CreatesPassedPawn(toRow, toCol, color))
            {
                bonusValue += 50;
            }
            
            // Bonus for central control
            if (toCol >= 2 && toCol <= 5) // Central files
            {
                bonusValue += 15;
            }
            
            // Penalty if it exposes king to danger (basic check)
            if (ExposesKingToDanger(fromRow, fromCol, toRow, toCol, color))
            {
                bonusValue -= 30;
            }
            
            return baseValue + bonusValue;
        }

        /// <summary>
        /// Executes an en passant move during AI evaluation with proper state management
        /// </summary>
        /// <param name="fromRow">Starting row</param>
        /// <param name="fromCol">Starting column</param>
        /// <param name="toRow">Target row</param>
        /// <param name="toCol">Target column</param>
        /// <param name="color">Color of moving pawn</param>
        /// <returns>Information needed to undo the move</returns>
        private EnPassantEvaluationInfo ExecuteEnPassantForEvaluation(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            var info = new EnPassantEvaluationInfo
            {
                FromRow = fromRow,
                FromCol = fromCol,
                ToRow = toRow,
                ToCol = toCol,
                MovingPiece = board[fromRow, fromCol],
                TargetSquareOriginal = board[toRow, toCol]
            };
            
            // Get victim information
            (int victimRow, int victimCol) = GetEnPassantVictimPosition();
            info.VictimRow = victimRow;
            info.VictimCol = victimCol;
            info.VictimPiece = board[victimRow, victimCol];
            
            // Execute the en passant capture
            board[toRow, toCol] = board[fromRow, fromCol];
            board[fromRow, fromCol] = ".";
            board[victimRow, victimCol] = ".";
            
            return info;
        }

        /// <summary>
        /// Undoes an en passant move after AI evaluation
        /// </summary>
        /// <param name="info">Information about the move to undo</param>
        private void UndoEnPassantForEvaluation(EnPassantEvaluationInfo info)
        {
            // Restore board state
            board[info.FromRow, info.FromCol] = info.MovingPiece;
            board[info.ToRow, info.ToCol] = info.TargetSquareOriginal;
            board[info.VictimRow, info.VictimCol] = info.VictimPiece;
        }

        /// <summary>
        /// Information needed to undo en passant moves during AI evaluation
        /// </summary>
        private class EnPassantEvaluationInfo
        {
            public int FromRow { get; set; }
            public int FromCol { get; set; }
            public int ToRow { get; set; }
            public int ToCol { get; set; }
            public int VictimRow { get; set; }
            public int VictimCol { get; set; }
            public string MovingPiece { get; set; } = "";
            public string TargetSquareOriginal { get; set; } = "";
            public string VictimPiece { get; set; } = "";
        }

        // ------------ AI EN PASSANT EVALUATION HELPERS ------------

        /// <summary>
        /// Checks if an en passant capture improves pawn structure
        /// </summary>
        private bool ImprovesePawnStructure(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            // Basic heuristic: moving to central files generally improves structure
            bool movesToCenter = toCol >= 2 && toCol <= 5;
            bool movesFromEdge = fromCol <= 1 || fromCol >= 6;
            
            // Bonus if moving from edge to center
            return movesToCenter && movesFromEdge;
        }

        /// <summary>
        /// Checks if an en passant capture creates a passed pawn
        /// </summary>
        private bool CreatesPassedPawn(int toRow, int toCol, string color)
        {
            // Simple check: if no enemy pawns ahead in same file or adjacent files
            string enemyColor = (color == "W") ? "B" : "W";
            int direction = (color == "W") ? -1 : 1;
            
            // Check files: left, center, right
            for (int fileOffset = -1; fileOffset <= 1; fileOffset++)
            {
                int checkCol = toCol + fileOffset;
                if (checkCol < 0 || checkCol >= 8) continue;
                
                // Check squares ahead
                for (int checkRow = toRow + direction; checkRow >= 0 && checkRow < 8; checkRow += direction)
                {
                    if (board[checkRow, checkCol] == enemyColor + "P")
                        return false; // Enemy pawn blocks passage
                }
            }
            
            return true; // No blocking pawns found
        }

        /// <summary>
        /// Checks if an en passant capture exposes the king to danger
        /// </summary>
        private bool ExposesKingToDanger(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            // Temporarily execute the en passant capture
            var info = ExecuteEnPassantForEvaluation(fromRow, fromCol, toRow, toCol, color);
            
            try
            {
                // Check if king is in check after the move
                bool kingInDanger = IsKingInCheck(color);
                return kingInDanger;
            }
            finally
            {
                // Always restore the board state
                UndoEnPassantForEvaluation(info);
            }
        }
        // Move logging
        private List<string> moveHistory = new List<string>();
        private int moveNumber = 1;
        
        // Undo/Redo functionality
        private Stack<GameState> gameStateHistory = new Stack<GameState>();
        private Stack<GameState> redoStack = new Stack<GameState>();

        /// <summary>
        /// Represents a complete game state for undo/redo functionality
        /// </summary>
        private class GameState
        {
            public string[,] Board { get; set; } = new string[8, 8];
            public string CurrentTurn { get; set; } = "";
            public bool WhiteKingMoved { get; set; }
            public bool BlackKingMoved { get; set; }
            public bool WhiteLeftRookMoved { get; set; }
            public bool WhiteRightRookMoved { get; set; }
            public bool BlackLeftRookMoved { get; set; }
            public bool BlackRightRookMoved { get; set; }
            public (int row, int col) EnPassantTarget { get; set; }
            public (int row, int col) EnPassantVictim { get; set; }
            public int MoveNumber { get; set; }
            public List<string> MoveHistory { get; set; } = new List<string>();
        }

        /// <summary>
        /// Saves the current game state for undo functionality
        /// </summary>
        private void SaveGameState()
        {
            var gameState = new GameState
            {
                CurrentTurn = currentTurn,
                WhiteKingMoved = whiteKingMoved,
                BlackKingMoved = blackKingMoved,
                WhiteLeftRookMoved = whiteLeftRookMoved,
                WhiteRightRookMoved = whiteRightRookMoved,
                BlackLeftRookMoved = blackLeftRookMoved,
                BlackRightRookMoved = blackRightRookMoved,
                EnPassantTarget = enPassantTarget,
                EnPassantVictim = enPassantVictim,
                MoveNumber = moveNumber,
                MoveHistory = new List<string>(moveHistory)
            };

            // Copy board state
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    gameState.Board[row, col] = board[row, col];
                }
            }

            gameStateHistory.Push(gameState);
            
            // Clear redo stack when a new move is made
            redoStack.Clear();
        }

        /// <summary>
        /// Undoes the last move
        /// </summary>
        public bool UndoMove()
        {
            if (gameStateHistory.Count == 0)
            {
                Console.WriteLine("No moves to undo.");
                return false;
            }

            // Save current state to redo stack
            var currentState = new GameState
            {
                CurrentTurn = currentTurn,
                WhiteKingMoved = whiteKingMoved,
                BlackKingMoved = blackKingMoved,
                WhiteLeftRookMoved = whiteLeftRookMoved,
                WhiteRightRookMoved = whiteRightRookMoved,
                BlackLeftRookMoved = blackLeftRookMoved,
                BlackRightRookMoved = blackRightRookMoved,
                EnPassantTarget = enPassantTarget,
                EnPassantVictim = enPassantVictim,
                MoveNumber = moveNumber,
                MoveHistory = new List<string>(moveHistory)
            };

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    currentState.Board[row, col] = board[row, col];
                }
            }

            redoStack.Push(currentState);

            // Restore previous state
            var previousState = gameStateHistory.Pop();
            RestoreGameState(previousState);

            Console.WriteLine("Move undone.");
            return true;
        }

        /// <summary>
        /// Redoes the last undone move
        /// </summary>
        public bool RedoMove()
        {
            if (redoStack.Count == 0)
            {
                Console.WriteLine("No moves to redo.");
                return false;
            }

            // Save current state to undo stack
            SaveGameState();

            // Restore redo state
            var redoState = redoStack.Pop();
            RestoreGameState(redoState);

            Console.WriteLine("Move redone.");
            return true;
        }

        /// <summary>
        /// Restores a game state
        /// </summary>
        private void RestoreGameState(GameState gameState)
        {
            currentTurn = gameState.CurrentTurn;
            whiteKingMoved = gameState.WhiteKingMoved;
            blackKingMoved = gameState.BlackKingMoved;
            whiteLeftRookMoved = gameState.WhiteLeftRookMoved;
            whiteRightRookMoved = gameState.WhiteRightRookMoved;
            blackLeftRookMoved = gameState.BlackLeftRookMoved;
            blackRightRookMoved = gameState.BlackRightRookMoved;
            enPassantTarget = gameState.EnPassantTarget;
            enPassantVictim = gameState.EnPassantVictim;
            moveNumber = gameState.MoveNumber;
            moveHistory = new List<string>(gameState.MoveHistory);

            // Restore board state
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    board[row, col] = gameState.Board[row, col];
                }
            }
        }

        /// <summary>
        /// Gets the number of moves that can be undone
        /// </summary>
        public int GetUndoCount()
        {
            return gameStateHistory.Count;
        }

        /// <summary>
        /// Gets the number of moves that can be redone
        /// </summary>
        public int GetRedoCount()
        {
            return redoStack.Count;
        }

        public string CurrentTurn()
        {
            return currentTurn;
        }

        /// <summary>
        /// Logs a move in algebraic notation
        /// </summary>
        private void LogMove(int fromRow, int fromCol, int toRow, int toCol, string pieceType, string color, bool isCapture = false, bool isEnPassant = false, bool isPromotion = false, char promotionPiece = 'Q')
        {
            string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
            string toSquare = GetAlgebraicNotation(toRow, toCol);
            
            string moveNotation = "";
            
            // Special moves
            if (isEnPassant)
            {
                moveNotation = $"{fromSquare}x{toSquare} e.p.";
            }
            else if (isPromotion)
            {
                string captureSymbol = isCapture ? "x" : "";
                moveNotation = $"{fromSquare}{captureSymbol}{toSquare}={promotionPiece}";
            }
            else
            {
                // Regular moves
                string pieceSymbol = pieceType == "P" ? "" : pieceType;
                string captureSymbol = isCapture ? "x" : "";
                moveNotation = $"{pieceSymbol}{fromSquare}{captureSymbol}{toSquare}";
            }
            
            // Add move to history
            if (color == "W")
            {
                moveHistory.Add($"{moveNumber}. {moveNotation}");
            }
            else
            {
                if (moveHistory.Count > 0)
                {
                    moveHistory[moveHistory.Count - 1] += $" {moveNotation}";
                }
                moveNumber++;
            }
            
            // Display the move
            Console.WriteLine($"Move: {moveNotation}");
        }
        
        /// <summary>
        /// Gets the complete move history
        /// </summary>
        public List<string> GetMoveHistory()
        {
            return new List<string>(moveHistory);
        }
        
        /// <summary>
        /// Prints the move history
        /// </summary>
        public void PrintMoveHistory()
        {
            Console.WriteLine("\n=== Move History ===");
            foreach (string move in moveHistory)
            {
                Console.WriteLine(move);
            }
            Console.WriteLine("==================\n");
        }

        public void PrintBoard()
        {
            Console.WriteLine();
            Console.WriteLine("    a   b   c   d   e   f   g   h");
            Console.WriteLine("  ┌───┬───┬───┬───┬───┬───┬───┬───┐");
            
            for (int row = 0; row < 8; row++)
            {
                Console.Write($"{8 - row} │");
                for (int col = 0; col < 8; col++)
                {
                    string piece = board[row, col];
                    if (piece == ".")
                    {
                        // Alternate between light and dark squares for better visualization
                        if ((row + col) % 2 == 0)
                            Console.Write(" ░ │"); // Light square
                        else
                            Console.Write(" ▓ │"); // Dark square
                    }
                    else
                    {
                        Console.Write($" {piece}│");
                    }
                }
                Console.WriteLine($" {8 - row}");
                
                // Print horizontal separator (except after last row)
                if (row < 7)
                {
                    Console.WriteLine("  ├───┼───┼───┼───┼───┼───┼───┼───┤");
                }
            }
            
            Console.WriteLine("  └───┴───┴───┴───┴───┴───┴───┴───┘");
            Console.WriteLine("    a   b   c   d   e   f   g   h");
            Console.WriteLine();
        }

        public (int, int) ParsePosition(string pos)
        {
            if (pos.Length != 2) return (-1, -1);
            int col = pos[0] - 'a';
            int row = 8 - (pos[1] - '0');
            if (col < 0 || col > 7 || row < 0 || row > 7) return (-1, -1);
            return (row, col);
        }

        /// <summary>
        /// Converts board coordinates to algebraic notation (e.g., e4)
        /// </summary>
        private string GetAlgebraicNotation(int row, int col)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8)
                return "Invalid";
            
            char file = (char)('a' + col);
            int rank = 8 - row;
            return $"{file}{rank}";
        }

        /// <summary>
        /// Gets the display name for a color
        /// </summary>
        private string GetColorName(string color)
        {
            return color == "W" ? "White" : "Black";
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
            bool isEnPassantCapture = false;
            string enPassantVictim = "";
            (int victimRow, int victimCol) = (-1, -1);

            // Check if this is an en passant capture
            string enPassantErrorMessage = string.Empty;
            bool isEnPassantAttempt = (type == "P" && captured == ".");
            
            if (isEnPassantAttempt && IsEnPassantCapture(fromRow, fromCol, toRow, toCol, color, out enPassantErrorMessage))
            {
                isEnPassantCapture = true;
                (victimRow, victimCol) = GetEnPassantVictimPosition();
                enPassantVictim = board[victimRow, victimCol];
            }
            else if (isEnPassantAttempt && !string.IsNullOrEmpty(enPassantErrorMessage) && 
                     Math.Abs(fromCol - toCol) == 1 && board[toRow, toCol] == ".")
            {
                // This looks like an attempted en passant that failed validation
                Console.WriteLine($"Invalid en passant: {enPassantErrorMessage}");
            }

            // Save game state before executing the move (for undo functionality)
            SaveGameState();

            // Execute the move
            board[toRow, toCol] = piece;
            board[fromRow, fromCol] = ".";

            // Execute en passant capture (remove victim pawn)
            if (isEnPassantCapture)
            {
                board[victimRow, victimCol] = ".";
                string colorName = GetColorName(color);
                string victimColor = GetColorName((color == "W") ? "B" : "W");
                string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
                string toSquare = GetAlgebraicNotation(toRow, toCol);
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                
                Console.WriteLine($"En passant capture! {colorName} pawn {fromSquare}-{toSquare} captures {victimColor} pawn on {victimSquare}.");
            }

            // Check if move leaves king in check
            bool kingInCheck = IsKingInCheck(color);
            if (kingInCheck)
            {
                // Undo the move
                board[fromRow, fromCol] = piece;
                board[toRow, toCol] = captured;
                
                // Undo en passant capture if applicable
                if (isEnPassantCapture)
                {
                    board[victimRow, victimCol] = enPassantVictim;
                }
                
                Console.WriteLine("You cannot move into check.");
                return;
            }

            // Handle pawn promotion after move validation but before turn switch
            if (type == "P" && RequiresPromotion(toRow, color))
            {
                // Human player promotion
                bool isAI = false;
                HandlePawnPromotion(toRow, toCol, color, isAI);
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

            // Update en passant state after successful move
            UpdateEnPassantState(fromRow, fromCol, toRow, toCol, type, color);

            currentTurn = currentTurn == "W" ? "B" : "W";

            if (IsCheckmate(currentTurn))
            {
                Console.WriteLine($"Checkmate! {color} wins!");
            }
        }

        public void MovePieceAI(int fromRow, int fromCol, int toRow, int toCol)
        {
            string piece = board[fromRow, fromCol];
            string color = piece[0].ToString();
            string type = piece[1].ToString();

            // AI moves are assumed to be valid since they come from MakeBestAIMove
            string captured = board[toRow, toCol];
            bool isEnPassantCapture = false;
            (int victimRow, int victimCol) = (-1, -1);

            // Check if this is an en passant capture
            if (type == "P" && captured == "." && IsEnPassantCapture(fromRow, fromCol, toRow, toCol, color))
            {
                isEnPassantCapture = true;
                (victimRow, victimCol) = GetEnPassantVictimPosition();
            }

            // Save game state before executing the AI move (for undo functionality)
            SaveGameState();

            // Execute the move
            board[toRow, toCol] = piece;
            board[fromRow, fromCol] = ".";

            // Execute en passant capture (remove victim pawn)
            if (isEnPassantCapture)
            {
                string victimPiece = board[victimRow, victimCol];
                board[victimRow, victimCol] = ".";
                
                // Provide user feedback for AI en passant
                string colorName = GetColorName(color);
                string victimColor = GetColorName((color == "W") ? "B" : "W");
                string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
                string toSquare = GetAlgebraicNotation(toRow, toCol);
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                
                Console.WriteLine($"AI {colorName} executes en passant! {fromSquare}-{toSquare} captures {victimColor} pawn on {victimSquare}.");
            }

            // Handle pawn promotion after move validation but before turn switch
            if (type == "P" && RequiresPromotion(toRow, color))
            {
                // AI promotion - automatically promote to Queen
                bool isAI = true;
                HandlePawnPromotion(toRow, toCol, color, isAI);
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

            // Update en passant state after successful move
            UpdateEnPassantState(fromRow, fromCol, toRow, toCol, type, color);

            // Log the AI move
            bool isCapture = captured != ".";
            bool isPromotion = (type == "P" && RequiresPromotion(toRow, color));
            char promotionPiece = 'Q'; // AI always promotes to Queen
            LogMove(fromRow, fromCol, toRow, toCol, type, color, isCapture, isEnPassantCapture, isPromotion, promotionPiece);

            currentTurn = currentTurn == "W" ? "B" : "W";

            if (IsCheckmate(currentTurn))
            {
                Console.WriteLine($"Checkmate! {color} wins!");
            }
        }

        // ------------ VALID MOVE CHECKS BELOW ------------

        private bool RequiresPromotion(int toRow, string color)
        {
            return (color == "W" && toRow == 0) || (color == "B" && toRow == 7);
        }

        private char GetPromotionChoice()
        {
            while (true)
            {
                Console.WriteLine("Pawn promotion! Choose a piece to promote to:");
                Console.WriteLine("Q - Queen");
                Console.WriteLine("R - Rook");
                Console.WriteLine("B - Bishop");
                Console.WriteLine("N - Knight");
                Console.Write("Enter your choice (Q/R/B/N): ");
                
                string input = Console.ReadLine()?.Trim().ToUpper();
                
                if (IsValidPromotionChoice(input, out char validChoice))
                {
                    return validChoice;
                }
                
                Console.WriteLine("Invalid choice. Please enter Q for Queen, R for Rook, B for Bishop, or N for Knight.");
            }
        }

        private bool IsValidPromotionChoice(string input, out char validChoice)
        {
            validChoice = '\0';
            
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            
            char choice = input.Trim().ToUpper()[0];
            
            if (choice == 'Q' || choice == 'R' || choice == 'B' || choice == 'N')
            {
                validChoice = choice;
                return true;
            }
            
            return false;
        }

        private string CreatePromotedPiece(string color, char pieceType)
        {
            // Validate color input
            if (color != "W" && color != "B")
            {
                throw new ArgumentException($"Invalid color: {color}. Must be 'W' or 'B'.");
            }
            
            // Validate piece type input
            char upperPieceType = char.ToUpper(pieceType);
            if (upperPieceType != 'Q' && upperPieceType != 'R' && upperPieceType != 'B' && upperPieceType != 'N')
            {
                throw new ArgumentException($"Invalid piece type: {pieceType}. Must be 'Q', 'R', 'B', or 'N'.");
            }
            
            // Create and return the promoted piece string
            return color + upperPieceType;
        }

        private void HandlePawnPromotion(int toRow, int toCol, string color, bool isAI)
        {
            char pieceType;
            
            if (isAI)
            {
                // AI automatically promotes to Queen
                pieceType = 'Q';
            }
            else
            {
                // Human player chooses piece
                pieceType = GetPromotionChoice();
            }
            
            // Create the promoted piece and place it on the board
            string promotedPiece = CreatePromotedPiece(color, pieceType);
            board[toRow, toCol] = promotedPiece;
            
            // Display promotion message
            string pieceName = pieceType switch
            {
                'Q' => "Queen",
                'R' => "Rook", 
                'B' => "Bishop",
                'N' => "Knight",
                _ => "Unknown"
            };
            
            string playerType = isAI ? "AI" : "Player";
            Console.WriteLine($"{playerType} promoted pawn to {pieceName}!");
        }

        private bool IsValidPawnMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            int dir = (color == "W") ? -1 : 1;
            int startRow = (color == "W") ? 6 : 1;

            // Standard forward moves (straight)
            if (fromCol == toCol && board[toRow, toCol] == ".")
            {
                // One square forward
                if (toRow - fromRow == dir) return true;
                // Two squares forward from starting position
                if (fromRow == startRow && toRow - fromRow == 2 * dir && board[fromRow + dir, toCol] == ".") return true;
            }

            // Diagonal captures (including en passant)
            if (Math.Abs(fromCol - toCol) == 1 && toRow - fromRow == dir)
            {
                // Regular diagonal capture
                if (board[toRow, toCol] != "." && board[toRow, toCol][0].ToString() != color)
                    return true;
                
                // En passant capture
                if (board[toRow, toCol] == "." && IsValidEnPassantMove(fromRow, fromCol, toRow, toCol, color))
                    return true;
            }

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
            // Check bounds first
            if (toRow < 0 || toRow >= 8 || toCol < 0 || toCol >= 8) return false;
            
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

        private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
        {
            if (depth == 0) return EvaluateBoard();

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
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

                                // Handle pawn promotion during minimax evaluation
                                if (type == "P" && RequiresPromotion(toRow, currentTurn))
                                {
                                    // AI automatically promotes to Queen for evaluation
                                    board[toRow, toCol] = CreatePromotedPiece(currentTurn, 'Q');
                                }

                                if (IsKingInCheck(currentTurn))
                                {
                                    board[fromRow, fromCol] = backupFrom;
                                    board[toRow, toCol] = backupTo;
                                    continue;
                                }

                                string prevTurn = currentTurn;
                                currentTurn = currentTurn == "W" ? "B" : "W";
                                int eval = Minimax(depth - 1, false, alpha, beta);
                                currentTurn = prevTurn;

                                board[fromRow, fromCol] = backupFrom;
                                board[toRow, toCol] = backupTo;

                                maxEval = Math.Max(maxEval, eval);
                                alpha = Math.Max(alpha, eval);
                                if (beta <= alpha) break;
                            }
                        }
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
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

                                // Handle pawn promotion during minimax evaluation
                                if (type == "P" && RequiresPromotion(toRow, currentTurn))
                                {
                                    // AI automatically promotes to Queen for evaluation
                                    board[toRow, toCol] = CreatePromotedPiece(currentTurn, 'Q');
                                }

                                if (IsKingInCheck(currentTurn))
                                {
                                    board[fromRow, fromCol] = backupFrom;
                                    board[toRow, toCol] = backupTo;
                                    continue;
                                }

                                string prevTurn = currentTurn;
                                currentTurn = currentTurn == "W" ? "B" : "W";
                                int eval = Minimax(depth - 1, true, alpha, beta);
                                currentTurn = prevTurn;

                                board[fromRow, fromCol] = backupFrom;
                                board[toRow, toCol] = backupTo;

                                minEval = Math.Min(minEval, eval);
                                beta = Math.Min(beta, eval);
                                if (beta <= alpha) break;
                            }
                        }
                    }
                }
                return minEval;
            }
        }

        // ------------ EN PASSANT STATE MANAGEMENT ------------

        /// <summary>
        /// Sets up an en passant opportunity when a pawn moves two squares forward
        /// </summary>
        /// <param name="pawnRow">Row where the pawn landed after two-square move</param>
        /// <param name="pawnCol">Column where the pawn landed after two-square move</param>
        private void SetEnPassantOpportunity(int pawnRow, int pawnCol)
        {
            // Calculate the target square (between start and end positions)
            int targetRow = (pawnRow == 3) ? 2 : 5; // White pawn creates target on rank 3, Black on rank 6
            enPassantTarget = (targetRow, pawnCol);
            enPassantVictim = (pawnRow, pawnCol);
        }

        /// <summary>
        /// Clears any existing en passant opportunity
        /// </summary>
        private void ClearEnPassantOpportunity()
        {
            enPassantTarget = (-1, -1);
            enPassantVictim = (-1, -1);
        }

        /// <summary>
        /// Checks if there is currently an en passant opportunity available
        /// </summary>
        /// <returns>True if en passant capture is possible</returns>
        private bool HasEnPassantOpportunity()
        {
            return enPassantTarget.row != -1 && enPassantTarget.col != -1;
        }

        /// <summary>
        /// Checks if the given target square is a valid en passant target
        /// </summary>
        /// <param name="toRow">Target row</param>
        /// <param name="toCol">Target column</param>
        /// <returns>True if the target matches the current en passant opportunity</returns>
        private bool IsValidEnPassantTarget(int toRow, int toCol)
        {
            return HasEnPassantOpportunity() && 
                   enPassantTarget.row == toRow && 
                   enPassantTarget.col == toCol;
        }

        /// <summary>
        /// Gets the position of the pawn that can be captured via en passant
        /// </summary>
        /// <returns>Tuple containing the row and column of the victim pawn</returns>
        private (int row, int col) GetEnPassantVictimPosition()
        {
            return enPassantVictim;
        }

        // ------------ EN PASSANT MOVE DETECTION ------------

        /// <summary>
        /// Checks if a move is an en passant capture attempt
        /// </summary>
        /// <param name="fromRow">Starting row of the moving pawn</param>
        /// <param name="fromCol">Starting column of the moving pawn</param>
        /// <param name="toRow">Target row</param>
        /// <param name="toCol">Target column</param>
        /// <param name="color">Color of the moving pawn</param>
        /// <param name="errorMessage">Output parameter for detailed error message if validation fails</param>
        /// <returns>True if this is an en passant capture attempt</returns>
        private bool IsEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // Must have an en passant opportunity available
            if (!HasEnPassantOpportunity())
            {
                errorMessage = "No en passant opportunity is currently available.";
                return false;
            }
            
            // Must be moving to the en passant target square
            if (!IsValidEnPassantTarget(toRow, toCol))
            {
                string targetSquare = GetAlgebraicNotation(enPassantTarget.row, enPassantTarget.col);
                string attemptedSquare = GetAlgebraicNotation(toRow, toCol);
                errorMessage = $"Square {attemptedSquare} is not a valid en passant target. Target is {targetSquare}.";
                return false;
            }
            
            // Must be a pawn making the move
            string piece = board[fromRow, fromCol];
            if (piece != color + "P")
            {
                string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
                errorMessage = $"Only pawns can execute en passant captures. Found {piece} at {fromSquare}.";
                return false;
            }
            
            // Must be a diagonal move (one square diagonally)
            int rowDiff = Math.Abs(toRow - fromRow);
            int colDiff = Math.Abs(toCol - fromCol);
            if (rowDiff != 1 || colDiff != 1)
            {
                errorMessage = "En passant must be a diagonal move (one square diagonally).";
                return false;
            }
            
            // Must be moving in the correct direction for the color
            int expectedDirection = (color == "W") ? -1 : 1;
            if ((toRow - fromRow) != expectedDirection)
            {
                string direction = (color == "W") ? "up" : "down";
                errorMessage = $"{GetColorName(color)} pawns must move {direction} the board.";
                return false;
            }
            
            // Must be adjacent to the victim pawn
            if (fromRow != enPassantVictim.row)
            {
                string victimSquare = GetAlgebraicNotation(enPassantVictim.row, enPassantVictim.col);
                errorMessage = $"Capturing pawn must be on the same rank as the victim pawn at {victimSquare}.";
                return false;
            }
            if (Math.Abs(fromCol - enPassantVictim.col) != 1)
            {
                string victimSquare = GetAlgebraicNotation(enPassantVictim.row, enPassantVictim.col);
                errorMessage = $"Capturing pawn must be adjacent to the victim pawn at {victimSquare}.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Checks if a move is an en passant capture attempt (simplified version)
        /// </summary>
        private bool IsEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            return IsEnPassantCapture(fromRow, fromCol, toRow, toCol, color, out _);
        }

        /// <summary>
        /// Validates all conditions for a valid en passant move
        /// </summary>
        /// <param name="fromRow">Starting row of the moving pawn</param>
        /// <param name="fromCol">Starting column of the moving pawn</param>
        /// <param name="toRow">Target row</param>
        /// <param name="toCol">Target column</param>
        /// <param name="color">Color of the moving pawn</param>
        /// <param name="errorMessage">Output parameter for detailed error message if validation fails</param>
        /// <returns>True if the en passant move is valid</returns>
        private bool IsValidEnPassantMove(int fromRow, int fromCol, int toRow, int toCol, string color, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // First check if this is an en passant capture attempt
            string captureError;
            if (!IsEnPassantCapture(fromRow, fromCol, toRow, toCol, color, out captureError))
            {
                errorMessage = $"Invalid en passant capture: {captureError}";
                return false;
            }
            
            // Verify the target square is empty (en passant target should always be empty)
            if (board[toRow, toCol] != ".")
            {
                string targetSquare = GetAlgebraicNotation(toRow, toCol);
                errorMessage = $"En passant target square {targetSquare} must be empty, found {board[toRow, toCol]} instead.";
                return false;
            }
            
            // Verify there's actually a victim pawn to capture
            string victimPiece = board[enPassantVictim.row, enPassantVictim.col];
            string opponentColor = (color == "W") ? "B" : "W";
            if (victimPiece != opponentColor + "P")
            {
                string victimSquare = GetAlgebraicNotation(enPassantVictim.row, enPassantVictim.col);
                errorMessage = $"Expected {GetColorName(opponentColor)} pawn at {victimSquare}, found {victimPiece} instead.";
                return false;
            }
            
            // Verify the moving pawn is on the correct rank for en passant
            int expectedRank = (color == "W") ? 3 : 4; // White pawns on rank 5, Black pawns on rank 4 (0-indexed)
            if (fromRow != expectedRank)
            {
                int expectedRankDisplay = (color == "W") ? 5 : 4; // Convert to 1-8 display format
                errorMessage = $"Pawn must be on rank {expectedRankDisplay} to execute en passant capture.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates all conditions for a valid en passant move (simplified version)
        /// </summary>
        private bool IsValidEnPassantMove(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            return IsValidEnPassantMove(fromRow, fromCol, toRow, toCol, color, out _);
        }

        /// <summary>
        /// Detects when a pawn move creates an en passant opportunity
        /// </summary>
        /// <param name="fromRow">Starting row of the pawn move</param>
        /// <param name="fromCol">Starting column of the pawn move</param>
        /// <param name="toRow">Ending row of the pawn move</param>
        /// <param name="toCol">Ending column of the pawn move</param>
        /// <param name="color">Color of the moving pawn</param>
        /// <returns>True if this move creates an en passant opportunity</returns>
        private bool CreatesEnPassantOpportunity(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            // Must be a pawn move
            string piece = board[fromRow, fromCol];
            if (piece != color + "P") return false;
            
            // Must be a two-square move
            if (Math.Abs(toRow - fromRow) != 2) return false;
            
            // Must be moving in the correct direction
            int expectedDirection = (color == "W") ? -1 : 1;
            if ((toRow - fromRow) != (2 * expectedDirection)) return false;
            
            // Must be moving from the starting rank
            int startingRank = (color == "W") ? 6 : 1; // White pawns start on rank 2, Black on rank 7 (0-indexed)
            if (fromRow != startingRank) return false;
            
            // Must be moving to the correct rank
            int targetRank = (color == "W") ? 4 : 3; // White moves to rank 4, Black to rank 5 (0-indexed)
            if (toRow != targetRank) return false;
            
            // Column must remain the same (straight move)
            if (fromCol != toCol) return false;
            
            return true;
        }

        /// <summary>
        /// Updates en passant state after a move is made
        /// </summary>
        /// <param name="fromRow">Starting row of the move</param>
        /// <param name="fromCol">Starting column of the move</param>
        /// <param name="toRow">Ending row of the move</param>
        /// <param name="toCol">Ending column of the move</param>
        /// <param name="pieceType">Type of piece that moved</param>
        /// <param name="color">Color of the piece that moved</param>
        private void UpdateEnPassantState(int fromRow, int fromCol, int toRow, int toCol, string pieceType, string color)
        {
            // Clear any existing en passant opportunity first (opportunities last only one turn)
            ClearEnPassantOpportunity();
            
            // Check if this move creates a new en passant opportunity
            if (pieceType == "P" && CreatesEnPassantOpportunity(fromRow, fromCol, toRow, toCol, color))
            {
                SetEnPassantOpportunity(toRow, toCol);
            }
        }

        // ------------ EN PASSANT TIMING MANAGEMENT ------------

        /// <summary>
        /// Validates that en passant timing rules are properly followed
        /// </summary>
        /// <param name="errorMessage">Output parameter for detailed error message if validation fails</param>
        /// <returns>True if en passant state is consistent with timing rules</returns>
        private bool ValidateEnPassantTiming(out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // If no en passant opportunity, timing is valid
            if (!HasEnPassantOpportunity()) return true;
            
            // Verify victim pawn is in correct position for en passant
            (int victimRow, int victimCol) = GetEnPassantVictimPosition();
            
            // Check bounds
            if (victimRow < 0 || victimRow >= 8 || victimCol < 0 || victimCol >= 8)
            {
                errorMessage = $"En passant victim position ({victimRow},{victimCol}) is out of bounds.";
                return false;
            }
            
            // Verify victim pawn exists and is on correct rank
            string victimPiece = board[victimRow, victimCol];
            if (victimPiece == ".")
            {
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                errorMessage = $"En passant victim pawn is missing at {victimSquare}.";
                return false;
            }
            
            // White pawns create opportunities on rank 4 (0-indexed), Black on rank 3
            bool isWhitePawn = victimPiece == "WP";
            bool isBlackPawn = victimPiece == "BP";
            bool correctRank = (isWhitePawn && victimRow == 4) || (isBlackPawn && victimRow == 3);
            
            if (!isWhitePawn && !isBlackPawn)
            {
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                errorMessage = $"En passant victim must be a pawn, found {victimPiece} at {victimSquare} instead.";
                return false;
            }
            
            if (!correctRank)
            {
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                int expectedRank = isWhitePawn ? 4 : 5; // Display rank (1-8)
                int actualRank = 8 - victimRow; // Convert to display rank
                errorMessage = $"{victimPiece} must be on rank {expectedRank} for valid en passant, found on rank {actualRank} at {victimSquare}.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates that en passant timing rules are properly followed (simplified version)
        /// </summary>
        private bool ValidateEnPassantTiming()
        {
            return ValidateEnPassantTiming(out _);
        }

        /// <summary>
        /// Forces clearing of en passant opportunities (for testing and edge cases)
        /// </summary>
        private void ForceExpireEnPassantOpportunity()
        {
            ClearEnPassantOpportunity();
        }

        /// <summary>
        /// Gets detailed information about current en passant state for debugging
        /// </summary>
        /// <returns>String describing current en passant state</returns>
        private string GetEnPassantStateInfo()
        {
            if (!HasEnPassantOpportunity())
                return "No en passant opportunity available";
            
            (int targetRow, int targetCol) = enPassantTarget;
            (int victimRow, int victimCol) = enPassantVictim;
            
            string victimPiece = (victimRow >= 0 && victimRow < 8 && victimCol >= 0 && victimCol < 8) 
                ? board[victimRow, victimCol] : "Invalid";
            
            string targetSquare = GetAlgebraicNotation(targetRow, targetCol);
            string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
            
            string validationStatus;
            string validationError;
            if (ValidateEnPassantConsistency(out validationError))
            {
                validationStatus = "Valid";
            }
            else
            {
                validationStatus = $"Invalid: {validationError}";
            }
            
            string captureColor = (victimPiece == "WP") ? "Black" : "White";
            string victimColor = (victimPiece == "WP") ? "White" : "Black";
            
            return $"En passant opportunity: {captureColor} can capture {victimColor} pawn\n" +
                   $"Target square: {targetSquare} ({targetRow},{targetCol})\n" +
                   $"Victim pawn: {victimSquare} ({victimRow},{victimCol}) = {victimPiece}\n" +
                   $"Status: {validationStatus}\n" +
                   $"Current turn: {currentTurn}";
        }

        /// <summary>
        /// Checks if an en passant opportunity should expire based on game state
        /// </summary>
        /// <param name="moveCount">Number of moves since opportunity was created</param>
        /// <returns>True if opportunity should expire</returns>
        private bool ShouldExpireEnPassantOpportunity(int moveCount)
        {
            // En passant opportunities expire after exactly one turn (2 half-moves)
            return moveCount >= 2;
        }

        /// <summary>
        /// Validates en passant state consistency across the entire board
        /// </summary>
        /// <param name="errorMessage">Output parameter for detailed error message if validation fails</param>
        /// <returns>True if en passant state is consistent with board state</returns>
        private bool ValidateEnPassantConsistency(out string errorMessage)
        {
            errorMessage = string.Empty;
            
            if (!HasEnPassantOpportunity()) return true;
            
            try
            {
                // Validate timing rules
                string timingError;
                if (!ValidateEnPassantTiming(out timingError))
                {
                    errorMessage = $"En passant timing validation failed: {timingError}";
                    return false;
                }
                
                // Validate target square is empty
                (int targetRow, int targetCol) = enPassantTarget;
                if (targetRow < 0 || targetRow >= 8 || targetCol < 0 || targetCol >= 8)
                {
                    errorMessage = $"En passant target position ({targetRow},{targetCol}) is out of bounds.";
                    return false;
                }
                
                if (board[targetRow, targetCol] != ".")
                {
                    string targetSquare = GetAlgebraicNotation(targetRow, targetCol);
                    errorMessage = $"En passant target square {targetSquare} must be empty, found {board[targetRow, targetCol]} instead.";
                    return false;
                }
                
                // Validate victim pawn exists and is correct type
                (int victimRow, int victimCol) = enPassantVictim;
                if (victimRow < 0 || victimRow >= 8 || victimCol < 0 || victimCol >= 8)
                {
                    errorMessage = $"En passant victim position ({victimRow},{victimCol}) is out of bounds.";
                    return false;
                }
                
                string victimPiece = board[victimRow, victimCol];
                if (victimPiece != "WP" && victimPiece != "BP")
                {
                    string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                    errorMessage = $"En passant victim must be a pawn, found {victimPiece} at {victimSquare} instead.";
                    return false;
                }
                
                // Validate geometric relationship between target and victim
                bool validGeometry = (Math.Abs(targetRow - victimRow) == 1) && (targetCol == victimCol);
                if (!validGeometry)
                {
                    string targetSquare = GetAlgebraicNotation(targetRow, targetCol);
                    string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                    errorMessage = $"Invalid geometric relationship between target {targetSquare} and victim {victimSquare}.";
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Unexpected error during en passant consistency validation: {ex.Message}";
                return false;
            }
        }
        
        /// <summary>
        /// Validates en passant state consistency across the entire board (simplified version)
        /// </summary>
        private bool ValidateEnPassantConsistency()
        {
            return ValidateEnPassantConsistency(out _);
        }

        // ------------ EN PASSANT CAPTURE EXECUTION ------------

        /// <summary>
        /// Executes an en passant capture with atomic board state updates
        /// </summary>
        /// <param name="fromRow">Starting row of the capturing pawn</param>
        /// <param name="fromCol">Starting column of the capturing pawn</param>
        /// <param name="toRow">Target row for the capturing pawn</param>
        /// <param name="toCol">Target column for the capturing pawn</param>
        /// <param name="color">Color of the capturing pawn</param>
        /// <returns>Information about the executed capture for potential rollback</returns>
        private EnPassantCaptureInfo ExecuteEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            // Validate that this is a valid en passant capture
            string errorMessage;
            if (!IsValidEnPassantMove(fromRow, fromCol, toRow, toCol, color, out errorMessage))
            {
                throw new InvalidOperationException($"Invalid en passant capture attempt: {errorMessage}");
            }

            // Get victim pawn information before capture
            (int victimRow, int victimCol) = GetEnPassantVictimPosition();
            string victimPiece = board[victimRow, victimCol];
            string capturingPiece = board[fromRow, fromCol];

            // Create capture info for potential rollback
            var captureInfo = new EnPassantCaptureInfo
            {
                CapturingPawnOriginalPos = (fromRow, fromCol),
                CapturingPawnFinalPos = (toRow, toCol),
                VictimPawnPos = (victimRow, victimCol),
                CapturingPiece = capturingPiece,
                VictimPiece = victimPiece,
                WasExecuted = false
            };

            try
            {
                // Execute the capture atomically
                // 1. Move the capturing pawn to target square
                board[toRow, toCol] = capturingPiece;
                board[fromRow, fromCol] = ".";

                // 2. Remove the victim pawn
                board[victimRow, victimCol] = ".";

                // 3. Mark capture as executed
                captureInfo.WasExecuted = true;

                // 4. Provide enhanced user feedback
                string victimColor = GetColorName((color == "W") ? "B" : "W");
                string colorName = GetColorName(color);
                string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
                string toSquare = GetAlgebraicNotation(toRow, toCol);
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                
                Console.WriteLine($"En passant capture executed! {colorName} pawn {fromSquare}-{toSquare} captures {victimColor} pawn on {victimSquare}.");

                return captureInfo;
            }
            catch (Exception ex)
            {
                // If anything goes wrong, ensure board state is consistent
                RollbackEnPassantCapture(captureInfo);
                throw new InvalidOperationException($"En passant capture execution failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Rolls back an en passant capture to restore the previous board state
        /// </summary>
        /// <param name="captureInfo">Information about the capture to rollback</param>
        private void RollbackEnPassantCapture(EnPassantCaptureInfo captureInfo)
        {
            if (!captureInfo.WasExecuted) return;

            try
            {
                // Restore the capturing pawn to its original position
                board[captureInfo.CapturingPawnOriginalPos.row, captureInfo.CapturingPawnOriginalPos.col] = captureInfo.CapturingPiece;
                
                // Clear the target square
                board[captureInfo.CapturingPawnFinalPos.row, captureInfo.CapturingPawnFinalPos.col] = ".";
                
                // Restore the victim pawn
                board[captureInfo.VictimPawnPos.row, captureInfo.VictimPawnPos.col] = captureInfo.VictimPiece;

                // Mark as rolled back
                captureInfo.WasExecuted = false;
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid cascading failures
                Console.WriteLine($"Warning: En passant rollback failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates that an en passant capture can be safely executed
        /// </summary>
        /// <param name="fromRow">Starting row of the capturing pawn</param>
        /// <param name="fromCol">Starting column of the capturing pawn</param>
        /// <param name="toRow">Target row for the capturing pawn</param>
        /// <param name="toCol">Target column for the capturing pawn</param>
        /// <param name="color">Color of the capturing pawn</param>
        /// <param name="errorMessage">Output parameter for detailed error message if validation fails</param>
        /// <returns>True if the capture can be executed safely</returns>
        private bool CanExecuteEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // Basic validation
            string validationError;
            if (!IsValidEnPassantMove(fromRow, fromCol, toRow, toCol, color, out validationError))
            {
                errorMessage = $"Invalid en passant move: {validationError}";
                return false;
            }

            // Check board boundaries
            if (fromRow < 0 || fromRow >= 8 || fromCol < 0 || fromCol >= 8 ||
                toRow < 0 || toRow >= 8 || toCol < 0 || toCol >= 8)
            {
                errorMessage = $"Invalid coordinates: from ({fromRow},{fromCol}) to ({toRow},{toCol}). Must be within board boundaries.";
                return false;
            }

            // Verify victim pawn position is valid
            (int victimRow, int victimCol) = GetEnPassantVictimPosition();
            if (victimRow < 0 || victimRow >= 8 || victimCol < 0 || victimCol >= 8)
            {
                errorMessage = $"Invalid victim pawn position: ({victimRow},{victimCol}). En passant state may be corrupted.";
                return false;
            }

            // Verify pieces are in expected positions
            string capturingPiece = board[fromRow, fromCol];
            string targetSquare = board[toRow, toCol];
            string victimPiece = board[victimRow, victimCol];

            if (capturingPiece != color + "P")
            {
                string fromSquare = GetAlgebraicNotation(fromRow, fromCol);
                errorMessage = $"Expected {GetColorName(color)} pawn at {fromSquare}, found {capturingPiece} instead.";
                return false;
            }
            
            if (targetSquare != ".")
            {
                string toSquare = GetAlgebraicNotation(toRow, toCol);
                errorMessage = $"Target square {toSquare} must be empty, found {targetSquare} instead.";
                return false;
            }
            
            string expectedVictimColor = (color == "W") ? "B" : "W";
            if (victimPiece != expectedVictimColor + "P")
            {
                string victimSquare = GetAlgebraicNotation(victimRow, victimCol);
                errorMessage = $"Expected {GetColorName(expectedVictimColor)} pawn at {victimSquare}, found {victimPiece} instead.";
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Validates that an en passant capture can be safely executed (simplified version)
        /// </summary>
        private bool CanExecuteEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)
        {
            return CanExecuteEnPassantCapture(fromRow, fromCol, toRow, toCol, color, out _);
        }

        /// <summary>
        /// Information about an en passant capture for rollback purposes
        /// </summary>
        private class EnPassantCaptureInfo
        {
            public (int row, int col) CapturingPawnOriginalPos { get; set; }
            public (int row, int col) CapturingPawnFinalPos { get; set; }
            public (int row, int col) VictimPawnPos { get; set; }
            public string CapturingPiece { get; set; } = "";
            public string VictimPiece { get; set; } = "";
            public bool WasExecuted { get; set; }
        }
    }
}
