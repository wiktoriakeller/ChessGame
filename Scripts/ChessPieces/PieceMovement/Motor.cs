using System.Collections;
using System;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public IEnumerator MoveTowardsTargetWithRotation(IMovable movementType, Vector3 targetPosition, Piece targetPiece, float movementSpeed, 
        float rotationSpeed, Action callback)
    {
        Vector3 previousRotation = transform.eulerAngles;
        Vector3 previousPosition = transform.position;

        yield return StartCoroutine(LookAtTargetOverTime(targetPosition, rotationSpeed));
        yield return StartCoroutine(movementType.MoveToTargetPosition(transform, targetPosition, movementSpeed));

        OnCollision(targetPiece, previousPosition, targetPosition);

        yield return StartCoroutine(LookAtAngle(previousRotation, rotationSpeed));

        callback?.Invoke();
    }

    public IEnumerator MoveTowardsTarget(IMovable movementType, Vector3 targetPosition, Piece targetPiece, float movementSpeed,
        float rotationSpeed, Action callback)
    {
        Vector3 previousRotation = transform.eulerAngles;
        Vector3 previousPosition = transform.position;

        yield return StartCoroutine(movementType.MoveToTargetPosition(transform, targetPosition, movementSpeed));
        OnCollision(targetPiece, previousPosition, targetPosition);
        callback?.Invoke();
    }

    private IEnumerator LookAtTargetOverTime(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 targetVec = (targetPosition - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(targetVec.z, targetVec.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }

        transform.eulerAngles = Vector3.up * targetAngle;
    }

    private IEnumerator LookAtAngle(Vector3 targetRotation, float rotationSpeed)
    {
        float targetAngle = targetRotation.y;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }

        transform.eulerAngles = Vector3.up * targetAngle;
    }

    private void OnCollision(Piece targetPiece, Vector3 startingPosition, Vector3 targetPosition)
    {
        Vector3 hitDirection = (targetPosition - startingPosition).normalized;
        targetPiece?.OnHit(hitDirection);
    }

    public void SetStandardRotationForBlackPiece()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}