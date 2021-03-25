using UnityEngine;

public struct PieceState
{
    public Vector2Int SquareIndex;
    public Vector2Int PreviousSquareIndex;
    public bool MovedForTheFirstTime;

    public PieceState(Vector2Int squareIndex, Vector2Int previousSquareIndex, bool movedFirstTime)
    {
        SquareIndex = squareIndex;
        PreviousSquareIndex = previousSquareIndex;
        MovedForTheFirstTime = movedFirstTime;
    }
}