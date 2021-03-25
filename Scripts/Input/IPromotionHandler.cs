using System;

public interface IPromotionHandler
{
    void PromotePiece(Piece piece, Action callback);
}
