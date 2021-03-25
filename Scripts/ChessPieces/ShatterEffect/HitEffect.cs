using UnityEngine;

public class HitEffect : MonoBehaviour, IHitable
{
    [SerializeField] private ParticleSystem effect;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    private Material effectMaterial;

    public void SetEffectMaterial(TeamColor color)
    {
        effectMaterial = color == TeamColor.White ? whiteMaterial : blackMaterial;
    }

    public void Play(Vector3 direction)
    {
        ParticleSystem particles = Instantiate(effect, transform.position + transform.localScale.z / 2 * direction, 
            Quaternion.FromToRotation(Vector3.forward, direction));
        particles.GetComponent<ParticleSystemRenderer>().material = effectMaterial;
        Destroy(particles.gameObject, particles.main.duration);
    }
}