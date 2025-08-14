using UnityEngine;

public class RaptorHearing : MonoBehaviour, IEnemyHearing
{
    [Header("Hearing Settings")]
    public float hearingRange = 5f; // How far the raptor can hear

    private RaptorController raptorController;

    void Awake()
    {
        raptorController = GetComponent<RaptorController>();
        if (raptorController == null)
        {
            Debug.LogError("RaptorHearing requires a RaptorController on the same GameObject.");
        }
    }

    public void HearSound(Vector3 soundPos, float volume)
    {
        if (raptorController == null) return;

        // Calculate distance to sound
        float distance = Vector3.Distance(transform.position, soundPos);

        if (distance <= hearingRange * volume)
        {

            // Set raptor to chase the noise
            raptorController.enabled = false; // temporarily disable AI updates
            StartCoroutine(MoveToNoise(soundPos));
        }
    }

    private System.Collections.IEnumerator MoveToNoise(Vector3 target)
    {
        // Enable NavMeshAgent manually
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.isStopped = false;
        agent.SetDestination(target);

        // Wait until raptor reaches the target
        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Resume normal AI
        raptorController.enabled = true;
    }
}

