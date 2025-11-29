using Chess.Classes.Abstract;
using ChessGame;
using System.Collections.Generic;

namespace Chess.Classes.FigureClasses
{
    public class Knight : BasicPiece
    {
        public Knight(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "Knight";
        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.KnightWhite
                : Properties.Resources.KnightBlack;
        }
        public override string GetSymbol() => Color == PieceColor.White ? "N" : "n";

        public override List<(int row, int col)> GetLegalMoves(ChessBoard board)
        {
            var moves = new List<(int, int)>();

            int[] dRow = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] dCol = { -1, 1, -2, 2, -2, 2, -1, 1 };

            // just add the same index direction == all leagal moves for the knight 
            for (int i = 0; i < 8; i++)
            {
                int r = Row + dRow[i];
                int c = Col + dCol[i];

                if (!board.IsInside(r, c))
                    continue;

                var target = board.GetPiece(r, c);
                if (target == null || target.Color != Color)
                    moves.Add((r, c));
            }

            return moves;
        }
    }
}
