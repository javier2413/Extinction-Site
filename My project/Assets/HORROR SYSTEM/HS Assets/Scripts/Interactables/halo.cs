using UnityEngine;

public class ToggleHaloOnProximity : MonoBehaviour
{
    private Behaviour halo;
    public string playerTag = "Player"; //for trigger to activate the "halo"

    private void Start()
    {
        // Findind the component
        Behaviour[] components = GetComponents<Behaviour>();
        foreach (var comp in components)
        {
            if (comp != null && comp.GetType().Name == "Halo")
            {
                halo = comp;
                halo.enabled = false; // Off at start
                break;
            }
        }

        if (halo == null)
        {
            Debug.LogWarning("No Halo component found on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (halo != null && other.CompareTag(playerTag))
        {
            halo.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (halo != null && other.CompareTag(playerTag))
        {
            halo.enabled = false;
        }
    }
}

