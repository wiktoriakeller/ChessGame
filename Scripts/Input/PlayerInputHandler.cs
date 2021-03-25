using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : IInputHandler, IInputReader
{
    public Vector3 SelectedPosition { get; private set; }
    private bool recievedInput;

    private bool piecePromotionIsActive;
    private Vector2Int promotedPieceSquare;
    private List<GameObject> pieceToChoose;
    private Action completePromotion;

    public PlayerInputHandler()
    {
        recievedInput = false;
        piecePromotionIsActive = false;
        pieceToChoose = new List<GameObject>();
    }

    public void ReadInput(Vector3 point, ChessGameController gameController)
    {
        if ((gameController.Board.TurnIsActive && !piecePromotionIsActive) || 
            (gameController.State != GameState.Normal && gameController.State != GameState.Check))
            return;

        recievedInput = true;
        SelectedPosition = point + new Vector3(Board.BOARD_OFFSET, 0, Board.BOARD_OFFSET);
    }

    public void HandleInput(ChessGameController gameController)
    {
        if (piecePromotionIsActive)
        {
            HandlePromotion(gameController);
            return;
        }

        Board board = gameController.Board;
        bool validPosition = board.CheckIfPositionIsValid(SelectedPosition);

        if (!validPosition)
            board.OnDeselectActivePiece();

        if (!recievedInput || !validPosition || board.TurnIsActive || (gameController.State != GameState.Normal && gameController.State != GameState.Check))
            return;

        recievedInput = false;
        Vector2Int chosenPosition = Board.CalculateSquareIndexFromBoardPosition(SelectedPosition);
        Piece chosenPiece = board.ChessGrid.GetPieceOnSquareIndex(chosenPosition);

        if(board.ChessGrid.SelectedPiece != null)
        {
            if (board.ChessGrid.SelectedPiece.CanMoveToSquare(chosenPosition))
            {
                board.OnSelectedPieceMove(board.ChessGrid.SelectedPiece.GetMoveFromSquareIndex(chosenPosition));
                gameController.OnEnteredNormalMode();
            }
            board.OnDeselectActivePiece();
        }
        else if(chosenPiece != null && chosenPiece.TeamColor == gameController.ActivePlayer.TeamColor)
        {
            board.OnSelectPiece(chosenPiece);
        }
    }

    public void PromotePiece(Piece piece, Action callback)
    {
        promotedPieceSquare = piece.SquareIndex;
        piecePromotionIsActive = true;
        completePromotion = callback;
        pieceToChoose.Clear();
        InitializePossiblePieces(piece);
    }

    private void HandlePromotion(ChessGameController gameController)
    {
        if (!recievedInput)
            return;

        bool foundPiece = false;
        GameObject selectedPiece = null;
        Vector2Int chosenPosition = Board.CalculateSquareIndexFromBoardPosition(SelectedPosition);
        
        foreach(var pieceObject in pieceToChoose)
        {
            if(Board.CalculateSquareIndexFromBoardPosition(pieceObject.transform.position) == chosenPosition)
            {
                foundPiece = true;
                selectedPiece = pieceObject;
                break;
            }
        }

        recievedInput = false;

        if (foundPiece)
        {
            ChessGameController.CreatePiecePrefabAndInitialize(promotedPieceSquare, selectedPiece.GetComponent<PieceEntity>().GetPieceType(), 
                gameController.ActivePlayer.TeamColor, gameController.Board.ChessGrid, gameController.ActivePlayer);

            foreach (var pieceObject in pieceToChoose)
            {
                pieceObject.GetComponent<PieceEntity>().Die();
            }

            piecePromotionIsActive = false;
            completePromotion();
        }
    }

    private void InitializePossiblePieces(Piece piece)
    {
        int pieceX = piece.SquareIndex.x;
        int pieceY = piece.SquareIndex.y;
        int direction = piece.TeamColor == TeamColor.White ? 1 : -1;

        Vector2Int[] positions = {piece.SquareIndex, new Vector2Int(pieceX, pieceY + direction), 
            new Vector2Int(pieceX - 1, pieceY + direction), new Vector2Int(pieceX + 1, pieceY + direction)};

        PieceType[] piecesToCreate = { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };

        for(int i = 0; i < piecesToCreate.Length; i++)
        {
            GameObject pieceObject = ChessGameController.PieceFactory.CreatePiecePrefab(piecesToCreate[i].ToString());
            pieceObject.GetComponent<PieceEntity>().InitializeEntity(Board.CalculateBoardPositionFromSquareIndex(positions[i]), piece.TeamColor);
            pieceToChoose.Add(pieceObject);
        }

        piece.OnDie();
    }
}