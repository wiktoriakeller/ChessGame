using System;
using UnityEngine;

[RequireComponent(typeof(ChessSquareSelector))]
public class Board : MonoBehaviour
{
    public const float BOARD_OFFSET = 0.5f;
    public const float SQUARE_SIZE = 1f;

    public ChessGridInfo ChessGrid { get; private set; }
    public ChessSquareSelector SquareSelector { get; private set; }
    public bool TurnIsActive { get; private set; }
    public event Action CompleteTurn;

    private ChessGameController gameController;
        
    private void Awake()
    {
        TurnIsActive = false;
        SquareSelector = GetComponent<ChessSquareSelector>();
        ChessGrid = new ChessGridInfo();
    }

    public void SetGameController(ChessGameController chessGameController)
    {
        gameController = chessGameController;
    }
    
    public void OnSelectPiece(Piece chosenPiece)
    {
        ChessGrid.SelectPiece(chosenPiece);
        SquareSelector.ActivateSquareSelectors(ChessGrid.SelectedPiece.AvailableMoves, ChessGrid.SelectedPiece);
    }

    public void OnDeselectActivePiece()
    {
        ChessGrid.DeselectPiece();
        SquareSelector.DeactivateSquareSelectors();
    }

    public static Vector3 CalculateBoardPositionFromSquareIndex(Vector2Int squareIndex)
    {
        return Vector3.right * SQUARE_SIZE * squareIndex.x + Vector3.forward * SQUARE_SIZE * squareIndex.y;
    }

    public static Vector2Int CalculateSquareIndexFromBoardPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / SQUARE_SIZE);
        int y = Mathf.FloorToInt(position.z / SQUARE_SIZE);
        return new Vector2Int(x, y);
    }

    public bool CheckIfPositionIsValid(Vector3 position)
    {
        if (ChessGrid.CheckIfSquareIndexIsValid(CalculateSquareIndexFromBoardPosition(position)))
            return true;
        return false;
    }

    public void OnSelectedPieceMove(Move move)
    {
        StartTurn();
        ChessGrid.MakeMove(move, gameController.ActivePlayer, gameController.GetOppositeToActivePlayer(), RotateToNewPlayer, HandlePromotion);
    }

    private void HandlePromotion()
    {
        gameController.ActivePlayer.HandlePromotion(ChessGrid.PieceMovedInPreviousTurn, RotateToNewPlayer);
    }

    private void RotateToNewPlayer()
    {
        StartCoroutine(gameController.CameraController.RotateToAnotherPlayer(EndTurn));
    }

    private void StartTurn()
    {
        TurnIsActive = true;
        ChessGrid.SaveMovedPiece();
    }

    private void EndTurn()
    {
        TurnIsActive = false;
        CompleteTurn?.Invoke();
    }
}