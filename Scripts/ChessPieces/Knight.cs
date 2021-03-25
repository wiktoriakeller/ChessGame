using UnityEngine;

public class Knight : Piece
{
    protected override int[,] possibleDirections { get; set; } = { { 2, 1 }, { 1, 2 }, { -1, 2 }, { -2, 1 }, 
        { -2, -1}, { -1, -2 }, { 1, -2 }, { 2, -1 } };

    public override void GeneratePossibleMoves(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy,
        ChessPlayer enemyCopy, bool checkMovesValidity, bool checkSpecialMoves)
    {
        AvailableMoves.Clear();
        Vector2Int newSquare = Vector2Int.zero;

        for (int i = 0; i < possibleDirections.GetLength(0); i++)
        {
            int newX = SquareIndex.x + possibleDirections[i, 0];
            int newY = SquareIndex.y + possibleDirections[i, 1];

            newSquare.Set(newX, newY);

            if (grid.CheckIfSquareIndexIsValid(newSquare))
            {
                Piece piece = grid.GetPieceOnSquareIndex(newSquare);
                Move move = new Move(this, newSquare, piece);

                if (checkMovesValidity && piece != null && piece.PieceType == PieceType.King)
                    continue;

                if (checkMovesValidity && (piece == null || (piece != null && piece.TeamColor != TeamColor)))
                {
                    Piece attackedPieceCopy = piece == null ? null : gridCopy.GetPieceOnSquareIndex(piece.SquareIndex);
                    Move moveCopy = new Move(gridCopy.GetPieceOnSquareIndex(SquareIndex), newSquare, attackedPieceCopy);
                    bool areMovesValid = CheckIfMoveIsValid(gridCopy, playerCopy, enemyCopy, moveCopy);

                    if(!areMovesValid)
                        continue;
                }

                if (piece == null)
                {
                    AvailableMoves.Add(move);
                }
                else if (piece != null && TeamColor != piece.TeamColor)
                {
                    AvailableMoves.Add(move);
                }
            }
        }
    }
}