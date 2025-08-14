using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    protected GameObject player;

    // Called when the player interacts with this object
    public virtual void Interact(GameObject player = null)
    {
        this.player = player;
        // Add additional behavior in child classes (like KeyInteraction)
    }

    // Convenience method to destroy this object after a delay
    protected void DestroySelf(float destroyTimer)
    {
        Destroy(this.gameObject, destroyTimer);
    }

    // Return the player that interacted with this object
    public GameObject GetPlayer()
    {
        return player;
    }
}

