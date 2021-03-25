using UnityEngine;

public class King : Knight
{
    protected override int[,] possibleDirections { get; set; } = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 },
        { 0, -1 }, { 1, -1 } };

    public override void GeneratePossibleMoves(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy, ChessPlayer enemyCopy, 
        bool checkMovesValidity, bool checkSpecialMoves)
    {
        base.GeneratePossibleMoves(grid, gridCopy, playerCopy, enemyCopy, checkMovesValidity, checkSpecialMoves);

        if (checkSpecialMoves)
            Castling(grid, gridCopy, enemyCopy);
    }

    private void Castling(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer enemyCopy)
    {
        if (MovedFirstTime)
            return;

        enemyCopy.GenerateAllPossibleMoves(gridCopy, false, false);

        if (enemyCopy.IsFieldUnderAttack(SquareIndex))
            return;

        int kingRow = SquareIndex.y;
        Piece leftRook = grid.GetPieceOnSquareIndex(new Vector2Int(0, kingRow));
        Piece rightRook = grid.GetPieceOnSquareIndex(new Vector2Int(ChessGridInfo.GRID_SIZE - 1, kingRow));

        if(leftRook != null && leftRook.PieceType == PieceType.Rook && leftRook.TeamColor == TeamColor && 
            !leftRook.MovedFirstTime && leftRook.SquareIndex.y == SquareIndex.y)
        {
            bool isLeftCastlingPossible = true;
            Vector2Int square = Vector2Int.zero;

            for (int i = leftRook.SquareIndex.x + 1; i < SquareIndex.x; i++)
            {
                square.Set(i, kingRow);
                if (gridCopy.GetPieceOnSquareIndex(square) != null || 
                    (i > 1 && enemyCopy.IsFieldUnderAttack(square)))
                {
                    isLeftCastlingPossible = false;
                    break;
                }
            }

            if (isLeftCastlingPossible)
            {
                Move move = new Move(this, new Vector2Int(2, kingRow), null, false, true, leftRook, new Vector2Int(3, kingRow));
                AvailableMoves.Add(move);
            }
        }

        if (rightRook != null && rightRook.PieceType == PieceType.Rook && rightRook.TeamColor == TeamColor 
            && !rightRook.MovedFirstTime && rightRook.SquareIndex.y == SquareIndex.y)
        {
            bool isRightCastlingPossible = true;
            Vector2Int square = Vector2Int.zero;

            for(int i = SquareIndex.x + 1; i < rightRook.SquareIndex.x; i++)
            {
                square.Set(i, kingRow);
                if (gridCopy.GetPieceOnSquareIndex(square) != null || enemyCopy.IsFieldUnderAttack(square))
                {
                    isRightCastlingPossible = false;
                    break;
                }
            }

            if (isRightCastlingPossible)
            {
                Move move = new Move(this, new Vector2Int(6, kingRow), null, false, true, rightRook, new Vector2Int(5, kingRow));
                AvailableMoves.Add(move);
            }
        }
    }
}