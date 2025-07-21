# ♟️ ChessEngine

A fully functional command-line chess engine written in C#. This project simulates a complete chess game between a human player and a computer-controlled AI opponent, implementing all major rules of chess with advanced gameplay logic and clean console UI.

---

## ✅ Major Features Implemented

### 1. ♙ Pawn Promotion System
- Complete pawn promotion to **Queen**, **Rook**, **Bishop**, or **Knight**
- Interactive user prompt for piece selection
- AI automatically promotes based on position evaluation
- Supports **multiple queens** and other advanced promotion logic
- Well-tested across multiple board states and scenarios

---

### 2. ✨ En Passant Implementation
- Accurate en passant capture logic, matching official chess rules
- Internal state tracking for en passant eligible pawns
- Ensures **correct timing** (only immediately after the opponent's double pawn push)
- Automatically handled in both player and AI turns
- Prevents invalid captures and maintains game integrity

---

### 3. 🤖 Enhanced Chess AI
- **Minimax Algorithm with Alpha-Beta Pruning** for better decision-making
- Dynamic board evaluation using:
  - Piece values
  - Centre control
  - King safety
  - Pawn structure analysis
- Realistic AI behaviour with pseudo "thinking delay"
- Configurable search depth (default: 3-ply)

---

### 4. 🧩 Game Management Features
- Full **algebraic move notation logging**
- **Undo / Redo** support with multiple levels
- Move history tracking
- Turn-based input flow for Human vs AI
- Built-in command support:
  - `undo` → revert previous move
  - `redo` → re-apply move
  - `history` → show past moves
  - `help` → command guide

---

### 5. 📋 Board Display & Console UI
- Clean and professional **ASCII board** with clear row/column labelling
- Proper spacing and character alignment
- Highlights current player's turn
- Consistent piece representation (e.g., `P`, `Q`, `K`, `R`, etc.)
- Simple and readable move input (e.g., `e2 e4`)

---
