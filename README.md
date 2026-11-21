# Chess Game (C# WinForms)
<p align="left">
  <img src="https://github.com/user-attachments/assets/a85bb265-e07e-424a-abf4-6def541f4277" width="300">
</p>

## Features
- Piece movement, check and checkmate logic
- Basic AI opponent
- WinForms UI
- OOP structure with:
  - abstraction (BasicPiece, LinePiece)
  - inheritance (Pawn, Knight, Rook, Bishop, Queen, King)
  - encapsulation of board and move logic

## AI
The AI looks at all legal moves, simulates each one, evaluates the position,
and chooses the move with the best score (material + small bonuses).



## Purpose and structure

A desktop learning project for representing chess pieces and board state with object-oriented C#. `Classes/Abstract` defines shared piece behavior, `Classes/FigureClasses` defines individual pieces, `ChessBoard` manages positions, and `Forms` contains the WinForms interface. `ChessAi` evaluates candidate moves with material scores and a check bonus.

## Build and run

Requires Windows and the .NET 8 SDK (or Visual Studio with .NET desktop development).

```powershell
dotnet build Chess.csproj
dotnet run --project Chess.csproj
```

## Validation and limitations

A build validates compilation, not complete chess-rule correctness. Manually check legal/illegal moves, check and checkmate positions, and AI turns. The repository currently has no automated rule suite. Castling, en passant, promotion and draw rules should be verified or implemented before describing this as a complete chess engine. The AI is a simple move-scoring opponent, not a deep-search engine.
