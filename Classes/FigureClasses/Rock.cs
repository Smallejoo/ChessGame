using Chess.Classes.Abstract;

namespace Chess.Classes.FigureClasses
{
    public class Rook : LinePiece
    {
        public Rook(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        public override string Name => "Rook";
        public override Image GetImage()
        {
            return Color == PieceColor.White
                ? Properties.Resources.RockWhite
                : Properties.Resources.RockBlack;
        }
        // can go many squares
        public override int MaxSteps => 7;

        // straight directions only
        public override (int dRow, int dCol)[] Directions => new[]
        {
            (1, 0),  // down
            (-1, 0), // up
            (0, 1),  // right
            (0, -1)  // left
        };
    }
}
