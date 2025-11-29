using Chess.Classes.Abstract;
using Chess.Classes.FigureClasses;
using System;
using System.Collections.Generic;

namespace ChessGame
{
    public class ChessAi
    {
        private readonly Random _rng = new Random();

        // how it works in a nutshell , get allur figures , check all there moves , you calculate 
        // each move posible and its score . 
        // so every figure will be moved to all its posible locations . no matter it ate some one or it didnt 
        // then on every move of it there will be a score for the board . from all this scores the best one will be picked 
        public bool MakeBestMove(ChessBoard board, PieceColor aiColor)
        {
            var bestMoves = new List<(BasicPiece piece, int row, int col)>();
            int bestScore = int.MinValue;

            var enemyColor = aiColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // get all your figures 
            foreach (var piece in board.GetAllPieces())
            {
                if (piece.Color != aiColor)
                    continue;
                // get all the moves for the current figure 
                foreach (var move in piece.GetLegalMoves(board))
                {
                    // where you are right now and where you wanna simulate the move 
                    int fromRow = piece.Row;
                    int fromCol = piece.Col;
                    int toRow = move.row;
                    int toCol = move.col;
                     
                    var captured = board.GetPiece(toRow, toCol);

                    //simulating move 
                    board.MovePiece(fromRow, fromCol, toRow, toCol);

                    // if after this move our king is in check bad -->  skip
                    if (board.IsKingInCheck(aiColor))
                    {
                        board.MovePiece(toRow, toCol, fromRow, fromCol);
                        board.SetPiece(toRow, toCol, captured);
                        continue;
                    }

                    // efter you made the move you could eat some one so u score the board 
                    int score = EvaluatePosition(board, aiColor);

                    // bonus !  if this move gives check to enemy
                    if (board.IsKingInCheck(enemyColor))
                        score += 50;

                    // bonus! capture value
                    if (captured != null)
                        score += GetPieceValue(captured);

                    //  undo simulation 
                    board.MovePiece(toRow, toCol, fromRow, fromCol);
                    board.SetPiece(toRow, toCol, captured);

                    // keep best moves (ties collected, we random between them)
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMoves.Clear();
                        bestMoves.Add((piece, toRow, toCol));
                    }
                    else if (score == bestScore)
                    {
                        bestMoves.Add((piece, toRow, toCol));
                    }
                }
            }

            if (bestMoves.Count == 0)
                return false; // no legal moves

            // pick random among the best-scoring ones
            var choice = bestMoves[_rng.Next(bestMoves.Count)];
            board.MovePiece(choice.piece.Row, choice.piece.Col, choice.row, choice.col);
            return true;
        }

        // Evaluate whole board: positive = good for AI, negative = good for enemy
        private int EvaluatePosition(ChessBoard board, PieceColor aiColor)
        {
            int score = 0;
            // just count all the figures on the board and sum there values . 
            foreach (var piece in board.GetAllPieces())
            {
                int value = GetPieceValue(piece);

                if (piece.Color == aiColor)
                    score += value;
                else
                    score -= value;
            }

            return score;
        }

        // Simple material values
        private int GetPieceValue(BasicPiece piece)
        {
            return piece switch
            {
                Pawn => 100,
                Knight => 300,
                Bishop => 300,
                Rook => 500,
                Queen => 900,
                King => 10000,
                _ => 0
            };
        }
    }
}
