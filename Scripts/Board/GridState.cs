
public struct GridState
{
    public Move MadeMove;
    public Piece PieceMovedInPreviousTurn;

    public GridState(Move madeMove, Piece pieceMovedInPreviousTurn)
    {
        MadeMove = madeMove;
        PieceMovedInPreviousTurn = pieceMovedInPreviousTurn;
    }
}