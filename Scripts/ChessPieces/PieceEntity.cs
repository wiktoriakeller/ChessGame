using System;
using UnityEngine;

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(HitEffect))]
public class PieceEntity : MonoBehaviour
{
    [SerializeField] private PieceProperties properties;
    [SerializeField] private PieceType pieceType;

    private Motor motor;
    private MaterialSetter materialSetter;
    private IHitable hitEffect;

    private void Awake()
    {
        motor = GetComponent<Motor>();
        materialSetter = GetComponent<MaterialSetter>();
        hitEffect = GetComponent<IHitable>();
    }

    public void InitializeEntity(Vector3 position, TeamColor color)
    {
        transform.position = position;
        materialSetter.SetTeamMaterial(color);
        hitEffect.SetEffectMaterial(color);

        if (color == TeamColor.Black)
            motor.SetStandardRotationForBlackPiece();
    }

    public PieceType GetPieceType()
    {
        return pieceType;
    }

    public void MoveEntity(IMovable movement, Vector3 newPosition, Piece target, Action callback)
    {
        if (properties.RotateOnMove)
        {
            MoveWithRotation(movement, newPosition, target, callback);
        }
        else
        {
            NormalMove(movement, newPosition, target, callback);
        }
    }

    private void NormalMove(IMovable movement, Vector3 newPosition, Piece target, Action callback)
    {
        StartCoroutine(motor.MoveTowardsTarget(movement, newPosition, target, properties.PieceSpeed, properties.RotationSpeed, callback));
    }

    private void MoveWithRotation(IMovable movement, Vector3 newPosition, Piece target, Action callback)
    {
        StartCoroutine(motor.MoveTowardsTargetWithRotation(movement, newPosition, target, properties.PieceSpeed, properties.RotationSpeed, callback));
    }

    public void PlayEffect(Vector3 direction)
    {
        hitEffect.Play(direction);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}