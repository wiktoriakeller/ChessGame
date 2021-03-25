using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetTeamMaterial(TeamColor color)
    {
        meshRenderer.material = color == TeamColor.White ? whiteMaterial : blackMaterial;
    }
}