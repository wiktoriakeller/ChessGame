using System.Collections.Generic;
using UnityEngine;

public class ChessSquareSelector : MonoBehaviour
{
    [SerializeField] private GameObject squareSelectorPrefab;
    [SerializeField] private Material checkMateSelecor;
    [SerializeField] private Material defaultSelector;
    [SerializeField] private Material onAttackSelector;
    [SerializeField] private Material onPieceChoseSelector;
    
    private GameObject[,] squareSelectors;
    private bool saveCheckMateSelector;
    private Vector2Int savedCheckMateSquare;

    private void Awake()
    {
        saveCheckMateSelector = false;
        savedCheckMateSquare = Vector2Int.zero;
        CreateSquareSelectors();
    }

    private void CreateSquareSelectors()
    {
        float upPosition = 0.0001f;
        squareSelectors = new GameObject[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

        for (int i = 0; i < ChessGridInfo.GRID_SIZE; i++)
        {
            for (int j = 0; j < ChessGridInfo.GRID_SIZE; j++)
            {
                squareSelectors[i, j] = Instantiate(squareSelectorPrefab);
                squareSelectors[i, j].transform.position = Vector3.right * j + Vector3.forward * i + Vector3.up * upPosition;
                squareSelectors[i, j].SetActive(false);
            }
        }
    }

    public void ActivateCheckMateSelector(Piece piece)
    {
        if(piece != null)
        {
            squareSelectors[piece.SquareIndex.y, piece.SquareIndex.x].GetComponent<MeshRenderer>().material = checkMateSelecor;
            squareSelectors[piece.SquareIndex.y, piece.SquareIndex.x].SetActive(true);
            saveCheckMateSelector = true;
            savedCheckMateSquare = piece.SquareIndex;
        }
    }

    public void DeactiavateCheckMateSelector()
    {
        saveCheckMateSelector = false;
        savedCheckMateSquare = Vector2Int.zero;
        DeactivateSquareSelectors();
    }

    public void ActivateSquareSelectors(List<Move> moves, Piece movingPiece)
    {
        if(movingPiece != null)
        {
            squareSelectors[movingPiece.SquareIndex.y, movingPiece.SquareIndex.x].SetActive(true);
            squareSelectors[movingPiece.SquareIndex.y, movingPiece.SquareIndex.x].GetComponent<MeshRenderer>().material = onPieceChoseSelector;
        }

        foreach (var move in moves)
        {
            squareSelectors[move.TargetSquare.y, move.TargetSquare.x].SetActive(true);
            
            if(move.TargetPiece != null)
                squareSelectors[move.TargetSquare.y, move.TargetSquare.x].GetComponent<MeshRenderer>().material = onAttackSelector;
        }
    }

    public void DeactivateSquareSelectors()
    {
        for (int i = 0; i < ChessGridInfo.GRID_SIZE; i++)
        {
            for (int j = 0; j < ChessGridInfo.GRID_SIZE; j++)
            {
                if (saveCheckMateSelector && i == savedCheckMateSquare.y && j == savedCheckMateSquare.x)
                {
                    squareSelectors[i, j].GetComponent<MeshRenderer>().material = checkMateSelecor;
                    continue;
                }

                squareSelectors[i, j].GetComponent<MeshRenderer>().material = defaultSelector;
                squareSelectors[i, j].SetActive(false);
            }
        }
    }
}