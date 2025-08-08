using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerB : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform player;
    public LayerMask obstructionLayer;
    public float detectionRadius = 10f;
    public float fovAngle = 120f;
    public float moveRadius = 15f;
    public float rotationSpeed = 5f;

    private bool isChasing = false;
    private bool isStunned = false;

    private Animator animator;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navAgent.updateRotation = false;  // We'll rotate manually
        navAgent.angularSpeed = 0;        // Disable automatic rotation

        Patrol();
    }

    void Update()
    {
        if (isStunned) return;

        if (CanSeePlayer())
        {
            if (!isChasing)
            {
                isChasing = true;
                animator.SetBool("Run", true);  // Start running animation
                animator.SetBool("Walk", false);
            }

            navAgent.isStopped = false;
            navAgent.SetDestination(player.position);
            RotateTowards(player.position);
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                animator.SetBool("Run", false);
                Patrol();
            }

            // If patrolling, check if destination reached and continue rotating to next patrol point
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                Patrol();
            }
            else
            {
                RotateTowards(navAgent.steeringTarget);
                animator.SetBool("Walk", true);
            }
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        float distance = Vector3.Distance(transform.position, player.position);

        if (angle < fovAngle / 2 && distance < detectionRadius)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distance, obstructionLayer))
            {
                return true;
            }
        }
        return false;
    }

    void Patrol()
    {
        Vector3 randomPoint = Random.insideUnitSphere * moveRadius + transform.position;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, moveRadius, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
            navAgent.isStopped = false;
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
    }

    public void Stun(float duration)
    {
        isStunned = true;
        navAgent.isStopped = true;
        animator.Play("React");
        Invoke(nameof(Unstun), duration);
    }

    void Unstun()
    {
        isStunned = false;
        navAgent.isStopped = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, moveRadius);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -fovAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fovAngle / 2, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * detectionRadius);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRadius);
    }
}




