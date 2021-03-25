using UnityEngine;

public interface IHitable
{
    void SetEffectMaterial(TeamColor color);
    void Play(Vector3 hitDirection);
}