using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomBoard : MonoBehaviour
{
    [SerializeField] private BoardLayout boardLayout;
    private List<PieceEntity> pieceEntities;
    private List<Vector2Int> squares;

    private void Awake()
    {
        pieceEntities = new List<PieceEntity>(ChessGridInfo.GRID_SIZE * 4);
        squares = new List<Vector2Int>(ChessGridInfo.GRID_SIZE * ChessGridInfo.GRID_SIZE);
        CreateSquares();
    }

    public void CreateRandomBoardLayout()
    {
        System.Random random = new System.Random();
        int numberOfSquares = ChessGridInfo.GRID_SIZE * ChessGridInfo.GRID_SIZE - 1;

        for (int i = 0; i < boardLayout.GetNumberOfPieces(); i++)
        {
            PieceType pieceType = boardLayout.GetChessPieceType(i);
            TeamColor teamColor = boardLayout.GetChessPieceTeamColor(i);

            int index = random.Next(0, numberOfSquares);
            PieceEntity other = pieceEntities.SingleOrDefault(x => 
                Board.CalculateSquareIndexFromBoardPosition(x.gameObject.transform.position) == squares[index]);
            pieceEntities.Remove(other);
            other?.Die();

            PieceEntity piece = ChessGameController.PieceFactory.CreatePiecePrefab(pieceType.ToString()).GetComponent<PieceEntity>();
            pieceEntities.Add(piece);
            piece.InitializeEntity(Board.CalculateBoardPositionFromSquareIndex(squares[index]), teamColor);
        }

    }

    public void Clear()
    {
        ClearBoardLayout();
    }
    
    private void ClearBoardLayout()
    {
        for(int i = 0; i < pieceEntities.Count; i++)
        {
            pieceEntities[i].Die();
        }

        pieceEntities.Clear();
    }

    private void CreateSquares()
    {
        for (int i = 0; i < ChessGridInfo.GRID_SIZE; i++)
        {
            for (int j = 0; j < ChessGridInfo.GRID_SIZE; j++)
            {
                squares.Add(new Vector2Int(j, i));
            }
        }
    }
}