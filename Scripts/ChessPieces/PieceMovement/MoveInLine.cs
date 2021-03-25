using System;
using System.Collections;
using UnityEngine;

public class MoveInLine : IMovable
{
    public IEnumerator MoveToTargetPosition(Transform objectTransform, Vector3 targetPosition, float speed)
    {
        while (objectTransform.position != targetPosition)
        {
            objectTransform.position = Vector3.MoveTowards(objectTransform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }
    }
}