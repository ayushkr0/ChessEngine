# Design Document

## Overview

The pawn promotion feature will be integrated into the existing ChessBoard class to handle the mandatory promotion of pawns when they reach the opponent's back rank. The design focuses on detecting promotion conditions, managing user interaction for piece selection, and ensuring proper game state updates.

## Architecture

The pawn promotion system will be implemented as an extension to the existing move validation and execution logic in the ChessBoard class. The architecture follows these key principles:

- **Detection**: Promotion conditions are checked during pawn move validation
- **Interaction**: Human players are prompted for piece selection, AI automatically promotes to Queen
- **State Management**: Board state is updated atomically to maintain game integrity
- **Integration**: Seamlessly integrates with existing move validation and game flow

## Components and Interfaces

### Core Components

#### 1. Promotion Detection
- **Location**: Enhanced `IsValidPawnMove` method in ChessBoard class
- **Responsibility**: Detect when a pawn move results in promotion
- **Integration**: Called during move validation in `MovePiece` method

#### 2. Promotion Handler
- **Location**: New `HandlePawnPromotion` method in ChessBoard class
- **Responsibility**: Orchestrate the promotion process
- **Parameters**: 
  - `int toRow, int toCol` - destination square
  - `string color` - pawn color
  - `bool isAI` - whether move is by AI

#### 3. Piece Selection Interface
- **Location**: New `GetPromotionChoice` method in ChessBoard class
- **Responsibility**: Handle user input for piece selection
- **Return**: Single character representing chosen piece ('Q', 'R', 'B', 'N')

#### 4. Board State Update
- **Location**: Enhanced `MovePiece` method in ChessBoard class
- **Responsibility**: Update board with promoted piece
- **Integration**: Replaces pawn with selected piece atomically

### Method Signatures

```csharp
// Detection and handling
private bool RequiresPromotion(int toRow, string color)
private void HandlePawnPromotion(int toRow, int toCol, string color, bool isAI)
private char GetPromotionChoice()
private string CreatePromotedPiece(string color, char pieceType)

// Validation enhancement
private bool IsValidPawnMove(int fromRow, int fromCol, int toRow, int toCol, string color) // Enhanced
```

## Data Models

### Piece Representation
The existing string-based piece representation will be maintained:
- Format: `{Color}{Type}` (e.g., "WQ", "BR", "BN")
- Promoted pieces follow same format as original pieces
- No distinction needed between original and promoted pieces

### Game State
No additional state variables required. Existing board array and turn tracking sufficient.

### Promotion Context
Temporary data during promotion process:
- Target square coordinates
- Pawn color
- Player type (human/AI)

## Error Handling

### Invalid Promotion Scenarios
- **Pawn not on promotion rank**: Handled by existing move validation
- **Invalid piece selection**: Prompt user to re-enter choice
- **System errors during promotion**: Restore previous board state

### User Input Validation
- Accept only valid piece characters: Q, R, B, N (case insensitive)
- Handle invalid input gracefully with re-prompting
- Provide clear instructions and error messages

### AI Promotion
- Always promote to Queen (no error conditions)
- No user interaction required
- Immediate game continuation

## Testing Strategy

### Unit Tests
1. **Promotion Detection**
   - White pawn reaching rank 8 triggers promotion
   - Black pawn reaching rank 1 triggers promotion
   - Pawns not on promotion rank do not trigger promotion

2. **Piece Selection**
   - Valid piece selections create correct pieces
   - Invalid selections are rejected
   - AI automatically selects Queen

3. **Board State Updates**
   - Promoted piece appears on correct square
   - Promoted piece has correct color
   - Original pawn is removed
   - Multiple Queens allowed on board

4. **Game Flow Integration**
   - Turn switches after promotion
   - Check/checkmate detection works with promoted pieces
   - AI can use promoted pieces in subsequent moves

### Integration Tests
1. **End-to-End Promotion Flow**
   - Complete human player promotion sequence
   - Complete AI promotion sequence
   - Promotion during check scenarios

2. **Game State Consistency**
   - Board evaluation includes promoted pieces
   - Move generation considers promoted pieces
   - Checkmate detection with promoted pieces

### Edge Cases
1. **Multiple Promotions**
   - Multiple Queens of same color
   - Promotion while in check
   - Promotion that delivers checkmate

2. **User Experience**
   - Clear promotion prompts
   - Graceful handling of invalid input
   - Consistent game flow

## Implementation Notes

### Integration Points
- Modify `MovePiece` method to check for promotion after valid pawn moves
- Enhance `IsValidPawnMove` to return promotion requirement flag
- Update AI move generation to handle promotion moves
- Ensure `EvaluateBoard` correctly values promoted pieces

### Performance Considerations
- Promotion detection adds minimal overhead to move validation
- User input for promotion pauses game naturally
- AI promotion is instantaneous
- No impact on minimax search performance

### Backward Compatibility
- Existing game functionality remains unchanged
- Board representation format maintained
- Move notation and parsing unaffected