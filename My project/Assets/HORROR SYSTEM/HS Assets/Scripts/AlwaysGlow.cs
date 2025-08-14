using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AlwaysGlow : MonoBehaviour
{
    private Renderer objRenderer;
    private Material objMaterial;

    [Header("Glow Settings")]
    public Color glowColor = Color.yellow;
    public float glowIntensity = 2f;

    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
        objMaterial = objRenderer.material;

        // Make sure the material supports emission
        objMaterial.EnableKeyword("_EMISSION");

        // Set emission color for constant glow
        objMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
    }
}

