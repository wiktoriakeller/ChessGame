using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece
{
    public Vector2Int SquareIndex { get; private set; }
    public Vector2Int PreviousSquareIndex { get; private set; }
    public PieceType PieceType { get; private set; }
    public TeamColor TeamColor { get; private set; }
    public bool MovedFirstTime { get; private set; }
    public IMovable Movement { get; private set; }
    public List<Move> AvailableMoves { get; private set; }
    public List<PieceState> States { get; private set; }

    protected abstract int[,] possibleDirections { get; set; }
    protected PieceEntity entity;

    public abstract void GeneratePossibleMoves(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy, ChessPlayer enemyCopy, 
        bool checkMovesValidity, bool checkSpecialMoves);

    public static bool operator ==(Piece p1, Piece p2)
    {
        if ((object)p1 == null)
            return (object)p2 == null;

        return p1.Equals(p2);
    }

    public static bool operator !=(Piece p1, Piece p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Piece)obj;

        if (SquareIndex == other.SquareIndex && PreviousSquareIndex == other.PreviousSquareIndex
            && PieceType == other.PieceType && TeamColor == other.TeamColor && MovedFirstTime == other.MovedFirstTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return (SquareIndex, PreviousSquareIndex, TeamColor, PieceType, MovedFirstTime).GetHashCode();
    }

    public virtual void SetData(Vector2Int squareIndex, Vector2Int previousSquareIndex, PieceType pieceType, TeamColor teamColor, bool movedFirstTime,
        IMovable movementType, PieceEntity pieceEntity)
    {
        SquareIndex = squareIndex;
        PreviousSquareIndex = previousSquareIndex;
        PieceType = pieceType;
        TeamColor = teamColor;
        Movement = movementType;
        entity = pieceEntity;
        entity?.InitializeEntity(Board.CalculateBoardPositionFromSquareIndex(SquareIndex), TeamColor);
        MovedFirstTime = movedFirstTime;
        AvailableMoves = new List<Move>();
        States = new List<PieceState>();
    }

    public bool CheckIfMoveIsValid(ChessGridInfo grid, ChessPlayer player, ChessPlayer enemy, Move pieceMove)
    {
        grid.MakeMove(pieceMove, player, enemy, true);

        enemy.GenerateAllPossibleMoves(grid, false, false);
        Piece king = player.FindFirstPieceOfType(PieceType.King);
        bool isMoveValid = !(enemy.IsFieldUnderAttack(king.SquareIndex));

        grid.UndoMove(player, enemy);

        return isMoveValid;
    }

    public bool CanMoveToSquare(Vector2Int squareIndex)
    {
        if (AvailableMoves.Any(move => move.TargetSquare == squareIndex))
            return true;
        return false;
    }

    public Move GetMoveFromSquareIndex(Vector2Int squareIndex)
    {
        foreach (var move in AvailableMoves)
        {
            if (move.TargetSquare == squareIndex)
                return move;
        }

        return new Move();
    }

    public void MovePiece(Move move, Action callback)
    {
        SetNewSquare(move.TargetSquare);
        entity?.MoveEntity(Movement, Board.CalculateBoardPositionFromSquareIndex(move.TargetSquare), move.TargetPiece, callback);
    }

    private void SetNewSquare(Vector2Int newSquareIndex)
    {
        States.Add(new PieceState(SquareIndex, PreviousSquareIndex, MovedFirstTime));
        if (!MovedFirstTime)
            MovedFirstTime = true;

        PreviousSquareIndex = SquareIndex;
        SquareIndex = newSquareIndex;
    }

    public void UndoMove()
    {
        if(States.Count > 0)
        {
            PieceState previousState = States[States.Count - 1];

            SquareIndex = previousState.SquareIndex;
            PreviousSquareIndex = previousState.PreviousSquareIndex;
            MovedFirstTime = previousState.MovedForTheFirstTime;

            States.RemoveAt(States.Count - 1);
        }
    }

    public void OnHit(Vector3 hitDirection)
    {
        entity?.PlayEffect(hitDirection);
        OnDie();
    }

    public void OnDie()
    {
        entity?.Die();
    }
}