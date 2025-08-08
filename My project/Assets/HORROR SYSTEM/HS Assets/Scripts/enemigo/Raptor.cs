using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Raptor : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject Player;
    public GameObject GOCanvas;
    public float RadioDeteccion;
    public float AnguloFOV;
    public float VisionPerdida;
    public float moveRadious;
    public float rotationSpeed = 7f;

    public LayerMask playerLayer;
    public LayerMask ObscructionLayer;

    private bool SiguiendoJugador = false;
    private Animator animator;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;

    private bool isStunned = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();

        agent.updateRotation = false;
        Patrol();
    }

    void Update()
    {

        if (playerHealth != null && playerHealth.IsPlayerDead())
        {
            if (audioSource != null && audioSource.enabled)
                audioSource.enabled = false;

            agent.isStopped = true;
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
            return;
        }


        if (Player)
        {
            DetectarJugador();
        }

        if (SiguiendoJugador)
        {
            agent.destination = Player.transform.position;

           
            RotateTowardsPlayer();

            animator.SetBool("Run", true);
            animator.SetBool("Walk", false);
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Patrol();
            }

            agent.isStopped = false;

            RotateTowards(agent.velocity.normalized);

            animator.SetBool("Run", false);
            animator.SetBool("Walk", true);
        }

        
        if (agent.velocity.sqrMagnitude > 0.01f) 
        {
            Vector3 moveDirection = agent.velocity.normalized;
            moveDirection.y = 0f; 
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }


    private void DetectarJugador()
    {
        Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
        float AngleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (AngleToPlayer <= AnguloFOV / 2)  
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, ObscructionLayer))
            {
                if (distanceToPlayer <= RadioDeteccion)
                {
                    SiguiendoJugador = true;
                    CancelInvoke(nameof(DejarSeguir)); 
                }
                else if (SiguiendoJugador)
                {
                   
                    Invoke(nameof(DejarSeguir), VisionPerdida);
                }
            }
        }
    }

    void Patrol()
    {
        Vector3 randomPoint = Random.insideUnitSphere * moveRadious + transform.position;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, moveRadious, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void DejarSeguir()
    {
        SiguiendoJugador = false;
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.magnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }


    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void Stun(float duration)
    {
        if (isStunned) return; 

        isStunned = true;
        agent.isStopped = true;
        animator.SetTrigger("Stun");

        CancelInvoke(nameof(Unstun));
        Invoke(nameof(Unstun), duration);
    }

    void Unstun()
    {
        isStunned = false;
        agent.isStopped = false;
        animator.ResetTrigger("Stun");
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RadioDeteccion);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, moveRadious);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -AnguloFOV / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, AnguloFOV / 2, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * RadioDeteccion);
        Gizmos.DrawRay(transform.position, rightBoundary * RadioDeteccion);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            GOCanvas.SetActive(true);

            Destroy(collision.gameObject);
        }
    }
}
