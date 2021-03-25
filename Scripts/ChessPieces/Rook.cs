using UnityEngine;

public class Rook : Piece
{
    protected override int[,] possibleDirections { get; set; } = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

    public override void GeneratePossibleMoves(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy, ChessPlayer enemyCopy, 
        bool checkMovesValidity, bool checkSpecialMoves)
    {
        AvailableMoves.Clear();
        Vector2Int newSquare = Vector2Int.zero;

        for (int i = 0; i < possibleDirections.GetLength(0); i++)
        {
            int currentX = SquareIndex.x;
            int currentY = SquareIndex.y;

            int newX = currentX + possibleDirections[i, 0];
            int newY = currentY + possibleDirections[i, 1];

            newSquare.Set(newX, newY);

            while (grid.CheckIfSquareIndexIsValid(newSquare))
            {
                Piece piece = grid.GetPieceOnSquareIndex(newSquare);
                Move move = new Move(this, newSquare, piece);

                if (checkMovesValidity && piece != null && piece.PieceType == PieceType.King)
                    break;

                if (checkMovesValidity && (piece == null || (piece != null && piece.TeamColor != TeamColor)))
                {
                    Piece attackedPieceCopy = piece == null ? null : gridCopy.GetPieceOnSquareIndex(piece.SquareIndex);
                    Move moveCopy = new Move(gridCopy.GetPieceOnSquareIndex(SquareIndex), newSquare, attackedPieceCopy);
                    bool areMovesValid = CheckIfMoveIsValid(gridCopy, playerCopy, enemyCopy, moveCopy);

                    if (piece == null && !areMovesValid)
                    {
                        newSquare.x += possibleDirections[i, 0];
                        newSquare.y += possibleDirections[i, 1];
                        continue;
                    }
                    else if(!areMovesValid)
                    {
                        break;
                    }
                }

                if (piece == null)
                {
                    AvailableMoves.Add(move);
                }
                else if(piece != null && TeamColor != piece.TeamColor)
                {
                    AvailableMoves.Add(move);
                    break;
                }
                else
                {
                    break;
                }

                newSquare.x += possibleDirections[i, 0];
                newSquare.y += possibleDirections[i, 1];
            }
        }

    }
}