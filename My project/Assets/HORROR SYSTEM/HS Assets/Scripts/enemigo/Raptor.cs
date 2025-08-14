using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Raptor : MonoBehaviour
{
    [Header("Movement")]
    public NavMeshAgent agent;
    public Rigidbody rb;
    public GameObject Player;
    public GameObject GOCanvas;
    public float RadioDeteccion;
    public float AnguloFOV;
    public float VisionPerdida;
    public float moveRadious;
    public float rotationSpeed = 7f;

    public LayerMask ObscructionLayer;

    private bool SiguiendoJugador = false;
    private Animator animator;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;

    private bool isStunned = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // solo rotación manual
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        agent.updateRotation = false;
        Patrol();
    }

    void Update()
    {
        if (isStunned)
        {
            agent.isStopped = true;
            rb.velocity = Vector3.zero;
            SiguiendoJugador = false;
            return;
        }

        if (playerHealth != null && playerHealth.IsPlayerDead())
        {
            if (audioSource != null) audioSource.enabled = false;
            agent.isStopped = true;
            return;
        }

        if (Player) DetectarJugador();

        if (SiguiendoJugador)
        {
            agent.destination = Player.transform.position;
            RotateTowardsPlayer();
            if (animator) animator.SetBool("Walk", true);
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                Patrol();

            agent.isStopped = false;
            RotateTowards(agent.velocity.normalized);
            if (animator) animator.SetBool("Walk", agent.velocity.sqrMagnitude > 0.01f);
        }

        // Rotación suave
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
            agent.SetDestination(hit.position);
    }

    void DejarSeguir() => SiguiendoJugador = false;

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

    public void StunFromFlashlight(Light spotLight, float duration)
    {
        if (isStunned) return;

        isStunned = true;

        // Congela NavMesh + Rigidbody
        agent.isStopped = true;
        rb.velocity = Vector3.zero;

        // Desconecta el seguimiento
        SiguiendoJugador = false;

        // Desstun luego del tiempo
        CancelInvoke(nameof(Unstun));
        Invoke(nameof(Unstun), duration);

        Debug.Log($"{name} stunado por {duration} segundos!");
    }

    private void Unstun()
    {
        isStunned = false;
        agent.isStopped = false;
        Debug.Log($"{name} ya no está stunado!");
    }
}

