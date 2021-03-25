using System;
using System.Collections;
using UnityEngine;

public class Jump : IMovable
{
    public IEnumerator MoveToTargetPosition(Transform objectTransform, Vector3 targetPosition, float speed)
    {
        float gravity = -9.81f * 2;
        float height = 1.5f * objectTransform.localScale.y;
        Vector3 objectStartPosition = objectTransform.position;
        Vector3 displacementXZ = new Vector3(targetPosition.x - objectStartPosition.x, 0, targetPosition.z - objectStartPosition.z);
        float time = 2 * Mathf.Sqrt(-2 * height / gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;
        Vector3 initialVelocity = velocityY + velocityXZ;
        float elpasedTime = 0f;

        while(elpasedTime < time)
        {
            Vector3 displacement = initialVelocity * elpasedTime + Vector3.up * (gravity * elpasedTime * elpasedTime) / 2;
            Vector3 position = objectStartPosition + displacement;
            objectTransform.position = position;
            elpasedTime += Time.deltaTime;
            yield return null;
        }

        objectTransform.position = targetPosition;
    }
}