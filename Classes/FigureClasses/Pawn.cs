using Chess.Classes.Abstract;
using ChessGame;
using System.Collections.Generic;

namespace Chess.Classes.FigureClasses
{
    public class Pawn : BasicPiece
    {
        public Pawn(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "Pawn";
        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.PawnWhite
                : Properties.Resources.PawnBlack;
        }


        public override List<(int row, int col)> GetLegalMoves(ChessBoard board)
        {
            var moves = new List<(int, int)>();

            int dir = Color == PieceColor.White ? 1 : -1;   // white goes down, black up
            int startRow = Color == PieceColor.White ? 1 : 6;

            int oneRow = Row + dir;

            // forward 1
            if (board.IsInside(oneRow, Col) && board.GetPiece(oneRow, Col) == null)
            {
                moves.Add((oneRow, Col));

                // forward 2 from starting rank
                int twoRow = Row + 2 * dir;
                if (Row == startRow &&
                    board.IsInside(twoRow, Col) &&
                    board.GetPiece(twoRow, Col) == null)
                {
                    moves.Add((twoRow, Col));
                }
            }

            // captures (diagonal)
            int[] dc = { -1, 1 };
            foreach (int dCol in dc)
            {
                int r = Row + dir;
                int c = Col + dCol;
                if (board.IsInside(r, c))
                {
                    var target = board.GetPiece(r, c);
                    if (target != null && target.Color != Color)
                        moves.Add((r, c));
                }
            }

            return moves;
        }
    }
}
