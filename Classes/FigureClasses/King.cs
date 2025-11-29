using Chess.Classes.Abstract;

namespace Chess.Classes.FigureClasses
{
    public class King : LinePiece
    {
        public King(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "King";
        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.KingWhite
                : Properties.Resources.KingBlack;
        }
        // only 1 step
        public override int MaxSteps => 1;

        // same directions as queen, but limited by MaxSteps
        public override (int dRow, int dCol)[] Directions => new[]
        {
            (1, 0), (-1, 0), (0, 1), (0, -1),
            (1, 1), (1, -1), (-1, 1), (-1, -1)
        };
    }
}
