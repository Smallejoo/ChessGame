using Chess.Classes.Abstract;
using Chess.Classes.FigureClasses;
using System.CodeDom;
using System.Collections.Generic;

namespace ChessGame
{
    public class ChessBoard
    {
        private const int Size = 8;
        //it can have a board or null ? means null .  
        private readonly BasicPiece?[,] _board = new BasicPiece?[Size, Size];

        // can return the the piece on the board or it iwll return null . if there is nothing . 
        public BasicPiece? GetPiece(int row, int col) => _board[row, col];


        // just setting the piece on the board ...
        public void SetPiece(int row, int col, BasicPiece? piece)
        {
            _board[row, col] = piece;
            if (piece != null)
            {
                piece.Row = row;
                piece.Col = col;
            }
        }

        //really usefull function to check we are on the board . 
        public bool IsInside(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size;
        }

        // just transfering cords of the piece  from one place to other .
        public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            var piece = _board[fromRow, fromCol];
            _board[fromRow, fromCol] = null;

            if (piece != null)
            {
                piece.Row = toRow;
                piece.Col = toCol;
            }

            _board[toRow, toCol] = piece;
        }

        // just returning all pieces on the board as a list 
        public  List<BasicPiece> GetAllPieces()
        {
            List<BasicPiece> AllPieces= new List<BasicPiece> ();

            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                {
                    var p = _board[r, c];
                    if (p != null)
                        AllPieces.Add(p);
                }
            return AllPieces;
        }

        // did we got the king we want ? 
        public King? GetKing(PieceColor color)
        {
            foreach (var piece in GetAllPieces())
            {
                if (piece is King k && k.Color == color)
                    return k;
            }
            return null;
        }

        //checking is the specific squre is safe . 
        //gets all figures and checks all of the color you want if they can reach this squre 
        // when it finds the first one it returns true . 
        // how it checks it ? it just calculate all moves of the piece and checks is it the squre we sarch for .
        
        public bool IsSquareUnderAttack(int row, int col, PieceColor byColor)
        {
            foreach (var piece in GetAllPieces())
            {
                if (piece.Color != byColor)
                    continue;

                foreach (var move in piece.GetLegalMoves(this))
                {
                    if (move.row == row && move.col == col)
                        return true;
                }
            }
            return false;
        }

        //checking is king under attack via checking is the kings squre is under attack by the enemy team 
        public bool IsKingInCheck(PieceColor color)
        {
            var king = GetKing(color);
            if (king == null) return true; // king captured

            var enemy = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            return IsSquareUnderAttack(king.Row, king.Col, enemy);
        }


        // are there any moves that wont expose the king 
        //gets all figures and checks ur own team figures can they save the king 
        // if even one move found returns true  

        public bool HasAnyLegalMove(PieceColor color)
        {
            foreach (var piece in GetAllPieces())
            {
                if (piece.Color != color)
                    continue;

                foreach (var move in piece.GetLegalMoves(this))
                {
                    if (IsMoveSafeForKing(piece, move.row, move.col))
                        return true;
                }
            }
            return false;
        }

        //takes a pice moves it some where and checks if the king is safe 
        // so or you moved the king to a safe position 
        // or you moved a figure to block the king you simulate it and check is the king safe now . 
        // returns true or false if king is safe . 
        
        private bool IsMoveSafeForKing(BasicPiece piece, int toRow, int toCol)
        {
            int fromRow = piece.Row;
            int fromCol = piece.Col;
            var captured = GetPiece(toRow, toCol);

            // simulate
            MovePiece(fromRow, fromCol, toRow, toCol);
            bool inCheck = IsKingInCheck(piece.Color);

            // undo
            MovePiece(toRow, toCol, fromRow, fromCol);
            SetPiece(toRow, toCol, captured);

            return !inCheck;
        }

        //if we move the king some where can he escape ? 
        // calculate all kings moves and checks is he safe there 

        public bool CanKingEscape(PieceColor color)
        {
            var king = GetKing(color);
            if (king == null) return false;

            foreach (var move in king.GetLegalMoves(this))
            {
                // simulate move
                var saved = GetPiece(move.row, move.col);
                MovePiece(king.Row, king.Col, move.row, move.col);

                bool stillInCheck = IsKingInCheck(color);

                // undo move
                MovePiece(move.row, move.col, king.Row, king.Col);
                SetPiece(move.row, move.col, saved);

                if (!stillInCheck)
                    return true;
            }

            return false;
        }

        //checks if any figure of our team can save the king by blocking 
        //stops if found one . 
        //simulates and undoes the moves 

        public bool CanBlockOrCapture(PieceColor color)
        {
            var king = GetKing(color);
            if (king == null) return false;

            foreach (var piece in GetAllPieces())
            {
                if (piece.Color != color)
                    continue;

                foreach (var move in piece.GetLegalMoves(this))
                {
                    // simulate move
                    var saved = GetPiece(move.row, move.col);
                    MovePiece(piece.Row, piece.Col, move.row, move.col);

                    bool stillInCheck = IsKingInCheck(color);

                    // undo move
                    MovePiece(move.row, move.col, piece.Row, piece.Col);
                    SetPiece(move.row, move.col, saved);

                    if (!stillInCheck)
                        return true;
                }
            }

            return false;
        }

        //checks all conditions previuse functions we wrote , and returns true if game is lost 
        public bool IsCheckmate(PieceColor color)
        {
            if (!IsKingInCheck(color))
                return false;

            if (CanKingEscape(color))
                return false;

            if (CanBlockOrCapture(color))
                return false;

            return true;
        }

        // just initin the over all board for the game to start  

        public void SetupInitialPosition()
        {
            // nulling every thing . 
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    _board[r, c] = null;

            // pawns
            for (int c = 0; c < Size; c++)
            {
                SetPiece(1, c, new Pawn(PieceColor.White, 1, c));
                SetPiece(6, c, new Pawn(PieceColor.Black, 6, c));
            }

            // rooks
            SetPiece(0, 0, new Rook(PieceColor.White, 0, 0));
            SetPiece(0, 7, new Rook(PieceColor.White, 0, 7));
            SetPiece(7, 0, new Rook(PieceColor.Black, 7, 0));
            SetPiece(7, 7, new Rook(PieceColor.Black, 7, 7));

            // knights (horses)
            SetPiece(0, 1, new Knight(PieceColor.White, 0, 1));
            SetPiece(0, 6, new Knight(PieceColor.White, 0, 6));
            SetPiece(7, 1, new Knight(PieceColor.Black, 7, 1));
            SetPiece(7, 6, new Knight(PieceColor.Black, 7, 6));

            // bishops
            SetPiece(0, 2, new Bishop(PieceColor.White, 0, 2));
            SetPiece(0, 5, new Bishop(PieceColor.White, 0, 5));
            SetPiece(7, 2, new Bishop(PieceColor.Black, 7, 2));
            SetPiece(7, 5, new Bishop(PieceColor.Black, 7, 5));

            // queens
            SetPiece(0, 3, new Queen(PieceColor.White, 0, 3));
            SetPiece(7, 3, new Queen(PieceColor.Black, 7, 3));

            // kings
            SetPiece(0, 4, new King(PieceColor.White, 0, 4));
            SetPiece(7, 4, new King(PieceColor.Black, 7, 4));
        }
    }
}
