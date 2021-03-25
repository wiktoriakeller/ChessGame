using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessGridInfo
{
    public const int GRID_SIZE = 8;
    public Piece SelectedPiece { get; private set; }
    public Piece PieceMovedInPreviousTurn { get; private set; }
    
    private Piece[,] chessBoardGrid;
    private List<GridState> previousStates;

    public ChessGridInfo()
    {
        chessBoardGrid = new Piece[GRID_SIZE, GRID_SIZE];
        ResetGrid();
        SelectedPiece = null;
        PieceMovedInPreviousTurn = null;
        previousStates = new List<GridState>();
    }

    public static ChessGridInfo CopyGrid(ChessGridInfo grid, ChessPlayer whitePlayer, ChessPlayer blackPlayer)
    {
        ChessGridInfo newGrid = new ChessGridInfo();
        Vector2Int square = Vector2Int.zero;

        for(int i = 0; i < GRID_SIZE; i++)
        {
            for(int j = 0; j < GRID_SIZE; j++)
            {
                square.Set(j, i);
                Piece pieceOnGrid = grid.GetPieceOnSquareIndex(square);

                if(pieceOnGrid != null)
                {
                    Piece copy = ChessGameController.PieceFactory.CreatePiece(pieceOnGrid.GetType().ToString());
                    copy.SetData(pieceOnGrid.SquareIndex, pieceOnGrid.PreviousSquareIndex, pieceOnGrid.PieceType, pieceOnGrid.TeamColor,
                        pieceOnGrid.MovedFirstTime, pieceOnGrid.Movement, null);
                    newGrid.SetChessPieceOnGrid(copy, square);

                    if (copy.TeamColor == TeamColor.White)
                        whitePlayer?.AddPiece(copy);

                    if (copy.TeamColor == TeamColor.Black)
                        blackPlayer?.AddPiece(copy);
                }
            }
        }

        newGrid.SelectedPiece = grid.SelectedPiece;
        newGrid.PieceMovedInPreviousTurn = grid.PieceMovedInPreviousTurn;
        return newGrid;
    }

    public void ResetGrid()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                Piece piece = chessBoardGrid[i, j];
                piece?.OnDie();
                chessBoardGrid[i, j] = null;
            }
        }
    }

    public void AddState(Move move)
    {
        previousStates.Add(new GridState(move, PieceMovedInPreviousTurn));
    }

    public void UndoMove(ChessPlayer player, ChessPlayer enemy)
    {
        if(previousStates.Count > 0)
        {
            GridState state = previousStates[previousStates.Count - 1];
            PieceMovedInPreviousTurn = (previousStates.Count - 2) >= 0 ? previousStates[previousStates.Count - 2].PieceMovedInPreviousTurn : null;

            Piece movingPiece = state.MadeMove.MovingPiece;
            Piece targetPiece = state.MadeMove.TargetPiece;
            Piece castlingPiece = state.MadeMove.CastlingPiece;

            if (state.MadeMove.PromotionFlag)
            {
                Piece promotedPiece = GetPieceOnSquareIndex(state.MadeMove.TargetSquare);
                player.RemovePiece(promotedPiece);
                MovePieceToNewSquare(new Move(movingPiece, movingPiece.PreviousSquareIndex, null), false);
                movingPiece.UndoMove();
                player.AddPiece(movingPiece);
            }
            else
            {
                MovePieceToNewSquare(new Move(movingPiece, movingPiece.PreviousSquareIndex, null), false);
                movingPiece.UndoMove();
            }

            if (targetPiece != null)
            {
                SetChessPieceOnGrid(targetPiece, targetPiece.SquareIndex);
                enemy.AddPiece(targetPiece);
            }

            if (state.MadeMove.CastlingFlag)
            {
                MovePieceToNewSquare(new Move(castlingPiece, castlingPiece.PreviousSquareIndex, null), false);
                castlingPiece.UndoMove();
            }

            previousStates.RemoveAt(previousStates.Count - 1);
        }
    }

    public void SelectPiece(Piece piece)
    {
        SelectedPiece = piece;
    }

    public void DeselectPiece()
    {
        SelectedPiece = null;
    }

    public void SaveMovedPiece()
    {
        PieceMovedInPreviousTurn = SelectedPiece;
    }

    public bool CheckIfSquareIndexIsValid(Vector2Int squareIndex)
    {
        if (squareIndex.x >= 0 && squareIndex.x < GRID_SIZE && squareIndex.y >= 0 && squareIndex.y < GRID_SIZE)
            return true;

        return false;
    }

    public void MakeMove(Move move, ChessPlayer player, ChessPlayer enemy, bool saveSelectedPiece = false)
    {
        if (move.TargetPiece != null)
        {
            SetChessPieceOnGrid(null, move.TargetPiece.SquareIndex);
            enemy.RemovePiece(move.TargetPiece);
        }

        if (saveSelectedPiece)
        {
            SelectPiece(move.MovingPiece);
            SaveMovedPiece();
        }

        if (move.CastlingFlag)
        {
            Move castlingMove = new Move(move.CastlingPiece, (Vector2Int)move.CastlingTargetSquare, null);
            MovePieceToNewSquare(castlingMove);
            move.CastlingPiece.MovePiece(castlingMove, null);
        }

        if (move.PromotionFlag)
        {
            MovePieceToNewSquare(move);
            move.MovingPiece.MovePiece(move, null);
            Piece promotedPiece = ChessGameController.CreateChessPiece(move.TargetSquare, PieceType.Queen, this, player);
            promotedPiece.SetData(move.TargetSquare, move.TargetSquare, PieceType.Queen, move.MovingPiece.TeamColor, false, null, null);
            player.AddPiece(promotedPiece);
            player.RemovePiece(move.MovingPiece);
        }
        else
        {
            MovePieceToNewSquare(move);
            move.MovingPiece.MovePiece(move, null);
        }
    }

    public void MakeMove(Move move, ChessPlayer player, ChessPlayer enemy, 
        Action onRegularMoveCallback, Action onPromotionCallback, bool saveSelectedPiece = false)
    {
        if (move.TargetPiece != null)
        {
            SetChessPieceOnGrid(null, move.TargetPiece.SquareIndex);
            enemy.RemovePiece(move.TargetPiece);
        }

        if (saveSelectedPiece)
        {
           SelectPiece(move.MovingPiece);
           SaveMovedPiece();
        }

        MovePieceToNewSquare(move);

        if (move.CastlingFlag)
        {
            Move castlingMove = new Move(move.CastlingPiece, (Vector2Int)move.CastlingTargetSquare, null);
            MovePieceToNewSquare(castlingMove);
            move.CastlingPiece.MovePiece(castlingMove, null);
        }

        if (move.PromotionFlag)
        {
            move.MovingPiece.MovePiece(move, onPromotionCallback);
        }
        else
        {
            move.MovingPiece.MovePiece(move, onRegularMoveCallback);
        }
    }

    public void SetChessPieceOnGrid(Piece piece, Vector2Int newSquareIndex)
    {
        if (CheckIfSquareIndexIsValid(newSquareIndex))
            chessBoardGrid[newSquareIndex.y, newSquareIndex.x] = piece;
    }

    private void MovePieceToNewSquare(Move pieceMove, bool storeMove = true)
    {
        if(storeMove)
            AddState(pieceMove);

        Vector2Int oldSquare = pieceMove.MovingPiece.SquareIndex;
        SetChessPieceOnGrid(pieceMove.MovingPiece, pieceMove.TargetSquare);
        SetChessPieceOnGrid(null, oldSquare);
    }

    public Piece GetPieceOnSquareIndex(Vector2Int squareIndex)
    {
        if (CheckIfSquareIndexIsValid(squareIndex))
            return chessBoardGrid[squareIndex.y, squareIndex.x];

        return null;
    }

    public Piece GetPieceOnSquareIndex(Piece other)
    {
        if (other == null)
            return null;

        return GetPieceOnSquareIndex(other.SquareIndex);
    }
}