using ChessGame;
using System.Collections.Generic;

namespace Chess.Classes.Abstract
{
    public enum PieceColor
    {
        White,
        Black
    }
    public struct Position
    {
        public int Row;
        public int Col;

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
    // abstract class for all the figures 
    public abstract class BasicPiece
    {
        public PieceColor Color { get; }

        public int Row { get; set; }
        public int Col { get; set; }

        // i want each class to have a name for the figure 
        public abstract string Name { get; }

        // constructor 
        protected BasicPiece(PieceColor color, int row, int col)
        {
            Color = color;
            Row = row;
            Col = col;
        }

        // get the Figure first latter . if its white then Caps if its black then lowercase 
        public virtual string GetSymbol()
        {
            string s = string.IsNullOrEmpty(Name) ? "?" : Name[0].ToString();
            return Color == PieceColor.White ? s.ToUpper() : s.ToLower();
        }

        // can be overited in other classes and return image 
        public virtual Image GetImage()
        {
            return null;
        }

       //later on this function will need to calculate all leagal moves for this figure 
        public abstract List<(int row, int col)> GetLegalMoves(ChessBoard board);
    }
}
