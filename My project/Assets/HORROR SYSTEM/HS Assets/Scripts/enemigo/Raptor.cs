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
    bool SiguiendoJugador;

    public LayerMask playerLayer;
    public LayerMask ObscructionLayer;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Patrullaje();
    }

    void Update()
    {
        if (Player)
        {

            DetectarJugador();
        }


        if (SiguiendoJugador)
        {
            agent.destination = Player.transform.position;
        }


        if (!SiguiendoJugador && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Patrullaje();
        }
    }


    private void DetectarJugador()
    {

        Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;


        float AngleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);


        if (AngleToPlayer <= AnguloFOV)
        {

            float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);


            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, ObscructionLayer))
            {

                if (distanceToPlayer <= RadioDeteccion)
                {

                    Debug.Log("abuelita vio a Player");

                    SiguiendoJugador = true;
                }
                else
                {

                    if (SiguiendoJugador)
                    {

                        Invoke("DejarSeguir", VisionPerdida);
                    }
                }
            }
        }
    }


    void Patrullaje()
    {

        Vector3 randomPoint = Random.insideUnitSphere * moveRadious;
        randomPoint += transform.position;

        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, moveRadious, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    void DejarSeguir()
    {
        SiguiendoJugador = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RadioDeteccion);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, moveRadious);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -AnguloFOV, 0) * Vector3.forward;
        Vector3 rigtBoundary = Quaternion.Euler(0, AnguloFOV, 0) * Vector3.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * RadioDeteccion);
        Gizmos.DrawRay(transform.position, rigtBoundary * RadioDeteccion);
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
