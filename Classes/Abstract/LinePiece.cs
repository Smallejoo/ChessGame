using ChessGame;
using System.Collections.Generic;

namespace Chess.Classes.Abstract
{
    public abstract class LinePiece : BasicPiece
    {
        // how much can it go (rock or king ) 
        public abstract int MaxSteps { get; }

        //what direction this pieace will go left right up down  (-1,0)  (1,0)  (0 ,-1)  (0, 1)
        public abstract (int dRow, int dCol)[] Directions { get; }

        protected LinePiece(PieceColor color, int row, int col)
            : base(color, row, col)
        {
        }

        // calculates all  leagal moves 
        public override List<(int row, int col)> GetLegalMoves(ChessBoard board)
        {
            var moves = new List<(int, int)>();

            // every piece will have directions
            foreach (var (dRow, dCol) in Directions)
            {

                
                for (int step = 1; step <= MaxSteps; step++)
                {
                    // calculate from the corrent position the direction and the steps it can get 
                    // 
                    int newRow = this.Row + dRow * step;
                    int newCol = this.Col + dCol * step;

                    
                    if (!board.IsInside(newRow, newCol))
                        break;

                    var target = board.GetPiece(newRow, newCol);

                    if (target == null)
                    {
                        moves.Add((newRow, newCol)); // empty square
                    }
                    else
                    {
                        if (target.Color != Color)
                            moves.Add((newRow, newCol)); // capture
                        // stopes adding steps cos its like a wall  
                        break; // can't jump over IMPORTENT !!! 
                    }
                }
            }

            return moves;
        }
    }
}
