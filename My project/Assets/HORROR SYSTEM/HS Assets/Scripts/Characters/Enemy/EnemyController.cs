using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Visibility Settings")]
    public int visibleRange = 5;

    [Header("Combat Settings")]
    public GameObject handDamage;
    public float attackDistance = 1f;
    public float chaseDistance = 2f;
    public float rotationSpeed = 5f;

    public AudioSource enemySound;

    private Transform target;
    private Transform player;
    private NavMeshAgent navAgent;
    private EnemyAnimations enemyAnimations;
    private bool isAttacking = false;
    private bool isStun = false;
    private bool isRoaring = false;

    private void Start()
    {
        InitializeEnemy();
    }

    private void Update()
    {
        if (isStun) return;

        HandleTargeting();
        HandleMovement();
    }

    private void InitializeEnemy()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyAnimations = GetComponent<EnemyAnimations>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        handDamage.GetComponent<Collider>().enabled = false;
    }

    private void HandleMovement()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth.IsPlayerDead())
        {
            StopEnemy();
            return;
        }

        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            Debug.Log("Distance to player:" + distanceToPlayer);

            if (isAttacking)
            {
                RotateTowardsPlayer();
                return;
            }

            if (distanceToPlayer > chaseDistance)
            {
                if (!isRoaring)
                {
                    isRoaring = true;

                    // Stop movement during roar
                    navAgent.isStopped = true;

                    // Start chasing after delay
                    Invoke(nameof(StartChasing), 1.5f);
                }
               
            }
            else if (distanceToPlayer <= attackDistance)
            {
                navAgent.isStopped = true;
                enemyAnimations.StopWalking();
                isAttacking = true;
                Invoke("FinishAttack", 2.2f);
            }
            else
            {
           
                enemyAnimations.Walk();
                navAgent.isStopped = false;
                navAgent.SetDestination(target.position);
            }
        }
        else
        {
            StopEnemy();
        }
    }

    private void StartChasing()
    {
        isRoaring = false;

        enemyAnimations.Walk();
        navAgent.isStopped = false;
        navAgent.SetDestination(target.position);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleTargeting()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visibleRange)
        {
            target = player;
        }
        else
        {
            target = null;
            StopEnemy();
        }
    }

    public void StopEnemy()
    {
        enemyAnimations.StopWalking();
        navAgent.isStopped = true;
        navAgent.ResetPath();
    }

    private void FinishAttack()
    {
        isAttacking = false;

    }

    public void EnableAttackCollider()
    {
        handDamage.GetComponent<Collider>().enabled = true;
    }

    public void DisableAttackCollider()
    {
        handDamage.GetComponent<Collider>().enabled = false;
    }
    public void StunEnemy(float duration)
    {
        if (isStun) return; // stunned

        isStun = true;
        StopEnemy(); // stop movement
        enemyAnimations.ReactToLight(); // play stun animation

        Invoke(nameof(RecoverFromStun), duration);
    }

    private void RecoverFromStun()
    {
        isStun = false;
    }
    public void DisableEnemySound()
    {
        enemySound.Stop();
    }
}