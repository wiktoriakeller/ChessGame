using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiInput : IInputHandler
{
    private const int BLACK_VICTORY = -30000;
    private const int WHITE_VICTORY = 30000;
    private const int MAX_DEPTH = 3;

    private bool piecePromotionIsActive;
    private Piece pieceToPromote;
    private Action completePromotion;

    public AiInput()
    {
        piecePromotionIsActive = false;
    }

    public void HandleInput(ChessGameController gameController)
    {
        if (piecePromotionIsActive)
        {
            HandlePromotion(gameController);
            return;
        }

        ChessPlayer player = gameController.ActivePlayer;
        Board board = gameController.Board;

        if (board.TurnIsActive || (gameController.State != GameState.Normal && gameController.State != GameState.Check))
            return;

        ChessPlayer playerCopy = new ChessPlayer(null, player.TeamColor);
        ChessPlayer enemyCopy = new ChessPlayer(null, gameController.GetOppositeToActivePlayer().TeamColor);
        ChessGridInfo gridCopy = playerCopy.TeamColor == TeamColor.White ? ChessGridInfo.CopyGrid(board.ChessGrid, playerCopy, enemyCopy) :
            ChessGridInfo.CopyGrid(board.ChessGrid, enemyCopy, playerCopy);

        Move foundMove = FindBestMove(gridCopy, playerCopy, enemyCopy);
        Move bestMove = new Move(board.ChessGrid.GetPieceOnSquareIndex(foundMove.MovingPiece), foundMove.TargetSquare,
            board.ChessGrid.GetPieceOnSquareIndex(foundMove.TargetPiece), foundMove.PromotionFlag, foundMove.CastlingFlag,
            board.ChessGrid.GetPieceOnSquareIndex(foundMove.CastlingPiece), foundMove.CastlingTargetSquare);

        board.OnSelectPiece(bestMove.MovingPiece);
        board.OnSelectedPieceMove(bestMove);
        gameController.OnEnteredNormalMode();
        board.OnDeselectActivePiece();
    }

    private List<Move> OrderMoves(List<Move> playerMoves, List<Move> enemyMoves)
    {
        Move[] moves = playerMoves.ToArray();
        int[] scores = new int[playerMoves.Count];
        int score;

        for(int i = 0; i < playerMoves.Count; i++)
        {
            score = 0;

            if (playerMoves[i].TargetPiece != null)
                score -= (PiecePositionEvaluation.PieceValue[playerMoves[i].TargetPiece.PieceType] - 
                    PiecePositionEvaluation.PieceValue[playerMoves[i].MovingPiece.PieceType]);

            if (playerMoves[i].PromotionFlag)
                score -= PiecePositionEvaluation.PieceValue[PieceType.Queen];

            if (enemyMoves.Any(move => move.TargetSquare == playerMoves[i].TargetSquare))
                score += PiecePositionEvaluation.PieceValue[playerMoves[i].MovingPiece.PieceType];
        }

        Array.Sort(scores, moves);
        return moves.ToList();
    }

    private Move FindBestMove(ChessGridInfo grid, ChessPlayer player, ChessPlayer enemy)
    {
        bool isMaximazing = player.TeamColor == TeamColor.White ? true : false;
        int bestScore = player.TeamColor == TeamColor.White ? int.MinValue : int.MaxValue;

        player.GenerateAllPossibleMoves(grid, true, true);
        enemy.GenerateAllPossibleMoves(grid, false, false);
        List<Move> possibleMoves = OrderMoves(player.AllPossibleMoves, enemy.AllPossibleMoves);
        Move bestMove = possibleMoves[0];

        for(int i = 0; i < possibleMoves.Count; i++)
        {
            grid.MakeMove(possibleMoves[i], player, enemy, true);

            int score = MiniMax(grid, enemy, player, !isMaximazing, MAX_DEPTH, int.MinValue, int.MaxValue);

            if((isMaximazing && score > bestScore) || (!isMaximazing && score < bestScore))
            {
                bestScore = score;
                bestMove = possibleMoves[i];
            }

            grid.UndoMove(player, enemy);
        }

        return bestMove;
    }

    private int MiniMax(ChessGridInfo grid, ChessPlayer player, ChessPlayer enemy, bool isMaximazing, int depth, int alpha, int beta)
    {
        player.GenerateAllPossibleMoves(grid, true, true);
        enemy.GenerateAllPossibleMoves(grid, false, false);

        if (depth == 0 || !player.HasAnyMoves())
            return EvaluateGrid(grid, player, enemy);

        List<Move> possibleMoves = OrderMoves(player.AllPossibleMoves, enemy.AllPossibleMoves);

        if (isMaximazing)
        {
            int bestScore = int.MinValue;

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                grid.MakeMove(possibleMoves[i], player, enemy, true);

                int score = MiniMax(grid, enemy, player, !isMaximazing, depth - 1, alpha, beta);
                bestScore = Mathf.Max(score, bestScore);
                alpha = Mathf.Max(alpha, bestScore);

                grid.UndoMove(player, enemy);

                if (alpha <= beta)
                    break;
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                grid.MakeMove(possibleMoves[i], player, enemy, true);

                int score = MiniMax(grid, enemy, player, !isMaximazing, depth - 1, alpha, beta);
                bestScore = Mathf.Min(score, bestScore);
                beta = Mathf.Min(beta, bestScore);

                grid.UndoMove(player, enemy);

                if (alpha <= beta)
                    break;
            }

            return bestScore;
        }
    }

    private int EvaluateGrid(ChessGridInfo grid, ChessPlayer player, ChessPlayer enemy)
    {
        int score = 0;
        bool isKingInCheck = player.IsKingInCheck(enemy);

        if (!isKingInCheck && player.AllPossibleMoves.Count == 0)
            return 0;

        if (player.TeamColor == TeamColor.White && player.AllPossibleMoves.Count == 0 && isKingInCheck)
            return BLACK_VICTORY;

        if (player.TeamColor == TeamColor.Black && player.AllPossibleMoves.Count == 0 && isKingInCheck)
            return WHITE_VICTORY;

        Vector2Int square = Vector2Int.zero;

        for(int i = 0; i < ChessGridInfo.GRID_SIZE; i++)
        {
            for(int j = 0; j < ChessGridInfo.GRID_SIZE; j++)
            {
                square.Set(j, i);
                Piece piece = grid.GetPieceOnSquareIndex(square);

                if(piece != null)
                {
                    if(piece.TeamColor == TeamColor.White)
                    {
                        score += PiecePositionEvaluation.PieceValue[piece.PieceType];
                    }
                    else
                    {
                        score -= PiecePositionEvaluation.PieceValue[piece.PieceType];
                    }

                    score += PiecePositionEvaluation.PositionEvaluation[(piece.PieceType, piece.TeamColor)][i, j];
                }
            }
        }

        return score;
    }

    private void HandlePromotion(ChessGameController gameController)
    {
        ChessGameController.CreatePiecePrefabAndInitialize(pieceToPromote.SquareIndex, PieceType.Queen, pieceToPromote.TeamColor,
            gameController.Board.ChessGrid, gameController.ActivePlayer);
        pieceToPromote.OnDie();
        piecePromotionIsActive = false;
        completePromotion?.Invoke();
    }

    public void PromotePiece(Piece piece, Action callback)
    {
        piecePromotionIsActive = true;
        pieceToPromote = piece;
        completePromotion = callback;
    }
}