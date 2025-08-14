using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class RaptorController : MonoBehaviour, IEnemyHearing
{
    [Header("Movement")]
    public NavMeshAgent agent;
    public float walkSpeed = 3f;
    public float chaseSpeed = 6f;
    public float moveRadius = 10f;
    public float rotationSpeed = 7f;

    [Header("Detection")]
    public Transform player;
    public float detectionRadius = 10f;
    public float FOVAngle = 120f;
    public LayerMask obstructionLayer;

    [Header("Stun")]
    public float stunDuration = 15f; // hard freeze duration
    private bool isStunned = false;

    [HideInInspector] public bool isChasing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!player) player = GameObject.FindWithTag("Player")?.transform;
        agent.updateRotation = false;
        agent.speed = walkSpeed;
    }

    void Start()
    {
        Patrol();
    }

    void Update()
    {
        if (isStunned)
        {
            agent.velocity = Vector3.zero; // ensures hard stop
            return;
        }

        DetectPlayer();

        if (isChasing && player != null)
        {
            agent.speed = chaseSpeed;
            agent.destination = player.position;
            RotateTowards(player.position);
        }
        else
        {
            agent.speed = walkSpeed;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                Patrol();

            RotateTowards(transform.position + agent.velocity);
        }

        // Play walk animation whenever agent moves
        if (TryGetComponent<Animator>(out Animator anim))
            anim.SetBool("Walk", agent.velocity.magnitude > 0.1f);
    }

    #region Patrol & Detection
    void Patrol()
    {
        Vector3 randomPoint = Random.insideUnitSphere * moveRadius + transform.position;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, moveRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    public void DetectPlayer()
    {
        if (!player || isStunned) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle <= FOVAngle / 2 && distance <= detectionRadius &&
            !Physics.Raycast(transform.position, dirToPlayer, distance, obstructionLayer))
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }
    #endregion

    #region Rotation
    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    #endregion

    #region Stun
    public void Stun()
    {
        if (isStunned) return;

        isStunned = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // ensures immediate hard stop

        StartCoroutine(StunCoroutine(stunDuration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        agent.isStopped = false;
    }
    #endregion

    #region IEnemyHearing
    public void HearSound(Vector3 soundPos, float volume)
    {
        if (isStunned) return;

        isChasing = true;
        agent.destination = soundPos;
        CancelInvoke(nameof(StopChasing));
        Invoke(nameof(StopChasing), 3f);
    }

    private void StopChasing()
    {
        isChasing = false;
    }
    #endregion
}









