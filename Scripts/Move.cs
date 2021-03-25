using UnityEngine;

public struct Move 
{
    public Piece MovingPiece;
    public Vector2Int TargetSquare;
    public Piece TargetPiece;
    public bool PromotionFlag;
    public bool CastlingFlag;
    public Piece CastlingPiece;
    public Vector2Int? CastlingTargetSquare;

    public Move(Piece movingPiece, Vector2Int targetSquare, Piece targetPiece, bool promotionFlag = false,
        bool castlingFlag = false, Piece castlingPiece = null, Vector2Int? castlingTargetSquare = null)
    {
        MovingPiece = movingPiece;
        TargetSquare = targetSquare;
        TargetPiece = targetPiece;
        PromotionFlag = promotionFlag;
        CastlingFlag = castlingFlag;
        CastlingPiece = castlingPiece;
        CastlingTargetSquare = castlingTargetSquare;
    }

    public bool AreMovesEqual(Move other)
    {
        if (MovingPiece != other.MovingPiece || TargetSquare != other.TargetSquare || TargetPiece != other.TargetPiece
            || CastlingPiece != other.CastlingPiece || CastlingTargetSquare != other.CastlingTargetSquare)
        {
            return false;
        }

        return true;
    }
}