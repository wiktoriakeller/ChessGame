using UnityEngine;

public class Pawn : Piece
{
    protected override int[,] possibleDirections { get; set; } = { { 0, 1 }, { 0, 2 } };
    private readonly int[,] possibleAttacks = { { -1, 1 }, { 1, 1 } };
    private readonly int[,] enPassantMoves = { { -1, 1 }, { 1, 1 } };

    public override void SetData(Vector2Int squareIndex, Vector2Int previousSquareIndex, PieceType pieceType, TeamColor teamColor, bool movedFirstTime,
        IMovable movementType, PieceEntity pieceEntity)
    {
        base.SetData(squareIndex, previousSquareIndex, pieceType, teamColor, movedFirstTime, movementType, pieceEntity);

        if(TeamColor == TeamColor.Black)
            InverseDirectionOfMoves();
    }

    private void InverseDirectionOfMoves()
    {
        for(int i = 0; i < possibleDirections.GetLength(0); i++)
            possibleDirections[i, 1] *= -1;

        for (int i = 0; i < possibleAttacks.GetLength(0); i++)
            possibleAttacks[i, 1] *= -1;

        for (int i = 0; i < enPassantMoves.GetLength(0); i++)
            enPassantMoves[i, 1] *= -1;
    }

    public override void GeneratePossibleMoves(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy,
        ChessPlayer enemyCopy, bool checkMovesValidity, bool checkSpecialMoves)
    {
        AvailableMoves.Clear();
        Vector2Int newSquare = Vector2Int.zero;

        for(int i = 0; i < possibleDirections.GetLength(0); i++)
        {
            if (i == 1 && MovedFirstTime)
                break;

            int newX = SquareIndex.x + possibleDirections[i, 0];
            int newY = SquareIndex.y + possibleDirections[i, 1];

            newSquare.Set(newX, newY);

            if (grid.CheckIfSquareIndexIsValid(newSquare))
            {
                Piece piece = grid.GetPieceOnSquareIndex(newSquare);
                Move move = CheckPromotionFlag(newSquare, piece);

                if (checkMovesValidity && piece == null)
                {
                    Piece attackedPieceCopy = piece == null ? null : gridCopy.GetPieceOnSquareIndex(piece.SquareIndex);
                    Move moveCopy = new Move(gridCopy.GetPieceOnSquareIndex(SquareIndex), newSquare, attackedPieceCopy, move.PromotionFlag);
                    bool areMovesValid = CheckIfMoveIsValid(gridCopy, playerCopy, enemyCopy, moveCopy);

                    if (!areMovesValid)
                        continue;
                }

                if (piece == null)
                {
                    AvailableMoves.Add(move);
                }
                else
                {
                    break;
                }
            }
        }

        PieceAttack(grid, gridCopy, playerCopy, enemyCopy, checkMovesValidity);
        EnPassant(grid, gridCopy, playerCopy, enemyCopy, checkMovesValidity);
    }

    private void PieceAttack(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy,
        ChessPlayer enemyCopy, bool checkMovesValidity)
    {
        Vector2Int newSquare = Vector2Int.zero;
        for (int i = 0; i < possibleAttacks.GetLength(0); i++)
        {
            int newX = SquareIndex.x + possibleAttacks[i, 0];
            int newY = SquareIndex.y + possibleAttacks[i, 1];

            newSquare.Set(newX, newY);

            if (grid.CheckIfSquareIndexIsValid(newSquare))
            {
                Piece piece = grid.GetPieceOnSquareIndex(newSquare);
                Move move = CheckPromotionFlag(newSquare, piece);

                if (checkMovesValidity && piece != null && piece.PieceType == PieceType.King)
                    continue;

                if (checkMovesValidity && (piece != null && piece.TeamColor != TeamColor))
                {
                    Piece attackedPieceCopy = piece == null ? null : gridCopy.GetPieceOnSquareIndex(piece.SquareIndex);
                    Move moveCopy = new Move(gridCopy.GetPieceOnSquareIndex(SquareIndex), newSquare, attackedPieceCopy, move.PromotionFlag);
                    bool areMovesValid = CheckIfMoveIsValid(gridCopy, playerCopy, enemyCopy, moveCopy);

                    if (!areMovesValid)
                        continue;
                }

                if (piece != null && TeamColor != piece.TeamColor)
                    AvailableMoves.Add(move);
            }
        }
    }

    private void EnPassant(ChessGridInfo grid, ChessGridInfo gridCopy, ChessPlayer playerCopy,
        ChessPlayer enemyCopy, bool checkMovesValidity)
    {
        int[,] directions = { { -1, 0 }, { 1, 0 } };
        Vector2Int newSquare = Vector2Int.zero;

        for(int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = SquareIndex.x + directions[i, 0];
            int newY = SquareIndex.y + directions[i, 1];

            newSquare.Set(newX, newY);

            if (grid.CheckIfSquareIndexIsValid(newSquare))
            {
                Piece piece = grid.GetPieceOnSquareIndex(newSquare);
                Vector2Int square = new Vector2Int(SquareIndex.x + enPassantMoves[i, 0], SquareIndex.y + enPassantMoves[i, 1]);
                Move move = CheckPromotionFlag(square, piece);

                if (checkMovesValidity && piece != null && piece.PieceType == PieceType.Pawn && piece == grid.PieceMovedInPreviousTurn
                    && Mathf.Abs(piece.SquareIndex.y - piece.PreviousSquareIndex.y) == 2 && piece.SquareIndex.x == piece.PreviousSquareIndex.x)
                {
                    Piece attackedPieceCopy = piece == null ? null : gridCopy.GetPieceOnSquareIndex(piece.SquareIndex);
                    Move moveCopy = new Move(gridCopy.GetPieceOnSquareIndex(SquareIndex), square, attackedPieceCopy, move.PromotionFlag);
                    bool areMovesValid = CheckIfMoveIsValid(gridCopy, playerCopy, enemyCopy, moveCopy);

                    if (!areMovesValid)
                        continue;
                }   

                if (piece != null && piece.PieceType == PieceType.Pawn && piece == grid.PieceMovedInPreviousTurn
                    && Mathf.Abs(piece.SquareIndex.y - piece.PreviousSquareIndex.y) == 2 && piece.SquareIndex.x == piece.PreviousSquareIndex.x)
                {
                    AvailableMoves.Add(move);
                    break;
                }
                    
            }
        }
    }

    private Move CheckPromotionFlag(Vector2Int targetSquare, Piece targetPiece)
    {
        int promotionRow = TeamColor == TeamColor.White ? ChessGridInfo.GRID_SIZE - 1 : 0;
        if(targetSquare.y == promotionRow)
        {
            return new Move(this, targetSquare, targetPiece, true);
        }

        return new Move(this, targetSquare, targetPiece);
    }
}