# En Passant Design Document

## Overview

The En Passant feature will extend the existing pawn movement system to support the special en passant capture rule. This implementation will integrate seamlessly with the current chess engine architecture, building upon the existing pawn promotion system and move validation framework.

## Architecture

### High-Level Design
The En Passant system will consist of several key components:

1. **En Passant State Tracking** - Track which pawns are vulnerable to en passant capture
2. **Move Validation Extension** - Extend existing pawn move validation to include en passant
3. **Capture Execution** - Handle the special two-pawn removal during en passant
4. **AI Integration** - Include en passant moves in AI move generation and evaluation
5. **Game State Management** - Maintain en passant opportunities across turns

### Integration Points
- **ChessBoard Class**: Primary integration point for all en passant logic
- **MovePiece Method**: Extended to handle en passant captures
- **IsValidPawnMove Method**: Enhanced to validate en passant moves
- **MakeBestAIMove Method**: Updated to consider en passant opportunities
- **Game State**: Extended to track en passant opportunities

## Components and Interfaces

### 1. En Passant State Management

```csharp
// New fields in ChessBoard class
private (int row, int col) enPassantTarget = (-1, -1);  // Square where en passant capture can occur
private (int row, int col) enPassantVictim = (-1, -1);  // Pawn that can be captured via en passant

// Core methods
private void SetEnPassantOpportunity(int pawnRow, int pawnCol)
private void ClearEnPassantOpportunity()
private bool HasEnPassantOpportunity()
private bool IsValidEnPassantTarget(int toRow, int toCol)
```

### 2. Move Validation Extension

```csharp
// Enhanced pawn move validation
private bool IsValidEnPassantMove(int fromRow, int fromCol, int toRow, int toCol, string color)
private bool IsEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)

// Integration with existing validation
// IsValidPawnMove will be extended to check for en passant moves
```

### 3. Capture Execution System

```csharp
// En passant capture execution
private void ExecuteEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol, string color)
private void RemoveEnPassantVictim()

// Integration with MovePiece and MovePieceAI methods
```

### 4. AI Integration Components

```csharp
// AI move generation enhancement
private List<(int fromRow, int fromCol, int toRow, int toCol)> GetEnPassantMoves(string color)
private int EvaluateEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol)

// Integration with MakeBestAIMove method
```

## Data Models

### En Passant State Structure
```csharp
public struct EnPassantState
{
    public int TargetRow { get; set; }      // Row where capture occurs
    public int TargetCol { get; set; }      // Column where capture occurs  
    public int VictimRow { get; set; }      // Row of pawn to be captured
    public int VictimCol { get; set; }      // Column of pawn to be captured
    public bool IsActive { get; set; }      // Whether opportunity is currently available
}
```

### Move Information Extension
The existing move validation system will be extended to include en passant move type identification.

## Detailed Implementation Design

### 1. En Passant Detection Logic

**Trigger Conditions:**
- Opponent pawn moves exactly 2 squares forward from starting position
- Player's pawn is adjacent (same rank) to the moved pawn
- No other moves have been made since the two-square pawn move

**Implementation Approach:**
```csharp
private void UpdateEnPassantState(int fromRow, int fromCol, int toRow, int toCol, string pieceType)
{
    // Clear previous en passant opportunity
    ClearEnPassantOpportunity();
    
    // Check if this move creates new en passant opportunity
    if (pieceType == "P" && Math.Abs(toRow - fromRow) == 2)
    {
        // Set en passant target square (between start and end positions)
        int targetRow = (fromRow + toRow) / 2;
        enPassantTarget = (targetRow, toCol);
        enPassantVictim = (toRow, toCol);
    }
}
```

### 2. Move Validation Integration

**Enhanced Pawn Move Validation:**
```csharp
private bool IsValidPawnMove(int fromRow, int fromCol, int toRow, int toCol, string color)
{
    // Existing pawn move validation logic...
    
    // Check for en passant capture
    if (IsEnPassantCapture(fromRow, fromCol, toRow, toCol, color))
    {
        return IsValidEnPassantMove(fromRow, fromCol, toRow, toCol, color);
    }
    
    // Existing logic continues...
}

private bool IsValidEnPassantMove(int fromRow, int fromCol, int toRow, int toCol, string color)
{
    // Validate en passant conditions
    if (!HasEnPassantOpportunity()) return false;
    if (toRow != enPassantTarget.row || toCol != enPassantTarget.col) return false;
    
    // Validate pawn position and movement
    int direction = (color == "W") ? -1 : 1;
    if (toRow - fromRow != direction) return false;
    if (Math.Abs(toCol - fromCol) != 1) return false;
    
    // Validate adjacent pawn position
    if (fromRow != enPassantVictim.row) return false;
    if (Math.Abs(fromCol - enPassantVictim.col) != 1) return false;
    
    return true;
}
```

### 3. Capture Execution System

**En Passant Capture Process:**
```csharp
private void ExecuteEnPassantCapture(int fromRow, int fromCol, int toRow, int toCol)
{
    // Move the capturing pawn
    string capturingPawn = board[fromRow, fromCol];
    board[toRow, toCol] = capturingPawn;
    board[fromRow, fromCol] = ".";
    
    // Remove the captured pawn
    board[enPassantVictim.row, enPassantVictim.col] = ".";
    
    // Clear en passant opportunity
    ClearEnPassantOpportunity();
    
    // Provide user feedback
    Console.WriteLine("En passant capture executed!");
}
```

### 4. AI Integration Design

**Move Generation Enhancement:**
```csharp
// In MakeBestAIMove method, add en passant move consideration
private void GenerateEnPassantMoves(string color, List<Move> moves)
{
    if (!HasEnPassantOpportunity()) return;
    
    // Find pawns that can execute en passant
    for (int col = 0; col < 8; col++)
    {
        int pawnRow = enPassantVictim.row;
        if (board[pawnRow, col] == color + "P")
        {
            if (Math.Abs(col - enPassantVictim.col) == 1)
            {
                // This pawn can capture via en passant
                moves.Add(new Move(pawnRow, col, enPassantTarget.row, enPassantTarget.col, MoveType.EnPassant));
            }
        }
    }
}
```

**Evaluation Enhancement:**
```csharp
private int EvaluateEnPassantCapture()
{
    // En passant captures are generally favorable
    // - Removes opponent pawn (material gain)
    // - Often improves pawn structure
    // - Can create tactical opportunities
    return 100; // Same value as regular pawn capture
}
```

## Error Handling

### Validation Errors
- **Invalid En Passant Attempt**: Clear error message explaining why en passant is not available
- **Timing Errors**: Inform player that en passant opportunity has expired
- **Position Errors**: Validate pawn positions and movement directions

### State Consistency
- **Board State Validation**: Ensure both pawns are removed correctly during en passant
- **Turn Management**: Maintain proper turn switching after en passant captures
- **Check Validation**: Ensure en passant moves don't leave king in check

### Recovery Mechanisms
- **State Reset**: Clear invalid en passant opportunities
- **Move Rollback**: Ability to undo en passant moves if needed for AI evaluation
- **Consistency Checks**: Validate board state after en passant execution

## Testing Strategy

### Unit Tests
1. **En Passant Detection Tests**
   - Test two-square pawn move detection
   - Test adjacent pawn position validation
   - Test timing constraints (next move only)

2. **Move Validation Tests**
   - Test valid en passant scenarios
   - Test invalid en passant attempts
   - Test edge cases (board boundaries, piece positions)

3. **Capture Execution Tests**
   - Test correct pawn removal
   - Test board state updates
   - Test turn switching

### Integration Tests
1. **Game Flow Tests**
   - Test en passant within normal gameplay
   - Test interaction with other special moves
   - Test AI vs human en passant scenarios

2. **AI Integration Tests**
   - Test AI en passant move generation
   - Test AI en passant evaluation
   - Test AI en passant execution

### Edge Case Tests
1. **Multiple En Passant Opportunities**
2. **En Passant Under Check**
3. **En Passant Revealing Check**
4. **En Passant in Endgame Scenarios**

## Performance Considerations

### Efficiency Optimizations
- **State Tracking**: Minimal memory overhead with simple coordinate tracking
- **Validation Speed**: Fast boolean checks for en passant conditions
- **AI Performance**: Efficient move generation without significant overhead

### Memory Management
- **Minimal State**: Only track current en passant opportunity (not history)
- **Cleanup**: Automatic cleanup of expired opportunities
- **Integration**: Reuse existing board representation and validation framework

## Security and Validation

### Input Validation
- **Move Format**: Validate en passant moves use standard chess notation
- **Position Bounds**: Ensure all coordinates are within board boundaries
- **Piece Validation**: Verify correct piece types are involved in en passant

### Game State Integrity
- **Atomic Operations**: En passant capture as single atomic operation
- **Consistency Checks**: Validate board state before and after en passant
- **Rule Compliance**: Ensure strict adherence to chess rules

This design provides a comprehensive foundation for implementing en passant while maintaining integration with the existing chess engine architecture and following the same patterns established in the pawn promotion feature.