using Chess.Classes.Abstract;

namespace Chess.Classes.FigureClasses
{
    public class Queen : LinePiece
    {
        public Queen(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "Queen";
        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.QueenWhite
                : Properties.Resources.QueenBlack;
        }
        public override int MaxSteps => 7;

        // all 8 directions: straight + diagonal
        public override (int dRow, int dCol)[] Directions => new[]
        {
            (1, 0), (-1, 0), (0, 1), (0, -1), // straight
            (1, 1), (1, -1), (-1, 1), (-1, -1) // diagonals
        };
    }
}
