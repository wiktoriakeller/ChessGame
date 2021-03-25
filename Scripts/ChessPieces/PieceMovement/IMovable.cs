using System;
using System.Collections;
using UnityEngine;

public interface IMovable
{
    IEnumerator MoveToTargetPosition(Transform objectTransform, Vector3 targetPosition, float speed);
}