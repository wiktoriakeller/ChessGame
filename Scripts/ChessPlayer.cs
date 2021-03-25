using System;
using UnityEngine;
using System.Collections.Generic;

public class ChessPlayer
{
    public TeamColor TeamColor { get; private set; }
    public List<Piece> ActivePieces { get; private set; }
    public List<Move> AllPossibleMoves { get; private set; }
    public readonly IInputHandler InputHandler;

    public ChessPlayer(IInputHandler inputHandler, TeamColor teamColor)
    {
        AllPossibleMoves = new List<Move>();
        ActivePieces = new List<Piece>();
        InputHandler = inputHandler;
        TeamColor = teamColor;
    }

    public void AddPiece(Piece piece)
    {
        if(piece != null && !ActivePieces.Contains(piece) && piece.TeamColor == TeamColor)
            ActivePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (piece != null && ActivePieces.Contains(piece))
            ActivePieces.Remove(piece);
    }

    public void HandleInput(ChessGameController gameController)
    {
        if(gameController.ActivePlayer.TeamColor == TeamColor)
            InputHandler.HandleInput(gameController);
    }

    public void GenerateAllPossibleMoves(ChessGridInfo grid, bool checkMovesValidity, bool checkSpecialMoves)
    {
        ChessGridInfo gridCopy = null;
        ChessPlayer enemyCopy = null;
        ChessPlayer playerCopy = null;

        if (checkMovesValidity)
        {
            TeamColor opponentColor = TeamColor == TeamColor.White ? TeamColor.Black : TeamColor.White;
            playerCopy = new ChessPlayer(null, TeamColor);
            enemyCopy = new ChessPlayer(null, opponentColor);
            gridCopy = opponentColor == TeamColor.White ? ChessGridInfo.CopyGrid(grid, enemyCopy, playerCopy)
                : ChessGridInfo.CopyGrid(grid, playerCopy, enemyCopy);
        }

        AllPossibleMoves.Clear();
        for(int i = 0; i < ActivePieces.Count; i++)
        {
            ActivePieces[i].GeneratePossibleMoves(grid, gridCopy, playerCopy, enemyCopy, checkMovesValidity, checkSpecialMoves);
            AllPossibleMoves.AddRange(ActivePieces[i].AvailableMoves);
        }
    }

    public bool IsFieldUnderAttack(Vector2Int square)
    {
        for(int i = 0; i < AllPossibleMoves.Count; i++)
        {
            if (AllPossibleMoves[i].TargetSquare == square)
                return true;
        }

        return false;
    }

    public bool IsKingInCheck(ChessPlayer enemy)
    {
        Piece king = FindFirstPieceOfType(PieceType.King);
        return enemy.IsFieldUnderAttack(king.SquareIndex);
    }

    public Piece FindFirstPieceOfType(PieceType pieceType)
    {
        for(int i = 0; i < ActivePieces.Count; i++)
        {
            if (ActivePieces[i].PieceType == pieceType)
                return ActivePieces[i];
        }

        return null;
    }

    public bool HasAnyMoves()
    {
        if (AllPossibleMoves.Count > 0)
            return true;
        return false;
    }

    public void HandlePromotion(Piece piece, Action callback)
    {
        InputHandler.PromotePiece(piece, callback);
    }
}