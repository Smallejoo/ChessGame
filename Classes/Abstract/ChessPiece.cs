namespace Chess.Classes.Abstract
{
    public enum PieceColorx
    {
        White,
        Black
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public class ChessPiece
    {
        public PieceColor Color { get; }
        public PieceType Type { get; }

        public ChessPiece(PieceColor color, PieceType type)
        {
            Color = color;
            Type = type;
        }
    }
}
