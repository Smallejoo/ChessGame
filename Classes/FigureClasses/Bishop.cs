using Chess.Classes.Abstract;

namespace Chess.Classes.FigureClasses
{
    public class Bishop : LinePiece
    {
        public Bishop(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "Bishop";

        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.FizerWhite
                : Properties.Resources.FizerBlack;
        }
        public override int MaxSteps => 7;

        // diagonal directions only
        public override (int dRow, int dCol)[] Directions => new[]
        {
            (1, 1),
            (1, -1),
            (-1, 1),
            (-1, -1)
        };
    }
}
