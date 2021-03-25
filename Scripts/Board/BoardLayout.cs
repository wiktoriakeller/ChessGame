using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialBoardLayout", menuName = "ScriptableObject/InitialBoardLayout")]
public class BoardLayout : ScriptableObject
{
    [Serializable]
    private class ChessPieceData
    {
        public Vector2Int SquareIndex;
        public PieceType PieceType;
        public TeamColor PieceColor;
    }

    [SerializeField] private ChessPieceData[] chessPiecesData;

    public int GetNumberOfPieces()
    {
        return chessPiecesData.Length;
    }

    public Vector2Int GetChessPieceSquareIndex(int index)
    {
        return chessPiecesData[index].SquareIndex;
    }

    public PieceType GetChessPieceType(int index)
    {
        return chessPiecesData[index].PieceType;
    }

    public TeamColor GetChessPieceTeamColor(int index)
    {
        return chessPiecesData[index].PieceColor;
    }
}