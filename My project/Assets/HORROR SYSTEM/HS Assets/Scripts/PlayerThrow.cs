using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    public float soundRadius = 5f; // How far the sound travels

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ThrowRock();
        }
    }

    void ThrowRock()
    {
        if (rockPrefab == null || throwPoint == null) return;

        GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
        }

        // Notify nearby enemies
        Collider[] hitColliders = Physics.OverlapSphere(throwPoint.position, soundRadius);
        foreach (var col in hitColliders)
        {
            IEnemyHearing hearing = col.GetComponent<IEnemyHearing>();
            if (hearing != null)
            {
                hearing.HearSound(throwPoint.position, soundRadius);
            }
        }
    }
}



