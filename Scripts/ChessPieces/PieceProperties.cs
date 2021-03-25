using UnityEngine;

[CreateAssetMenu(fileName = "PieceProperties", menuName = "ScriptableObject/PieceProperties")]
public class PieceProperties : ScriptableObject
{
    [SerializeField] private float pieceSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool rotateOnMove;

    public float PieceSpeed { get { return pieceSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public bool RotateOnMove { get { return rotateOnMove; } }
}