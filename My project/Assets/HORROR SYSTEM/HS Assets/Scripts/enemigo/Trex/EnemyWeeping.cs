using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyWeeping : MonoBehaviour
{
   
    
        [Header("Referencias")]
        public Transform player;

        [Header("Configuración")]
        public float fieldOfView = 60f;
        public float detectionRadius = 10f;

        private NavMeshAgent agent;
        private bool hasDetectedPlayer = false;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (!hasDetectedPlayer)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= detectionRadius)
                {
                    hasDetectedPlayer = true;
                }
            }

            if (hasDetectedPlayer)
            {
                if (!IsPlayerLookingAtMe())
                {
                    agent.SetDestination(player.position);
                    Debug.Log("Siguiendo al jugador");
                }
                else
                {
                    agent.ResetPath(); // Se detiene si lo están mirando
                    Debug.Log("Detenido porque el jugador me está mirando");
                }
            }
            Debug.Log("Update del enemigo está corriendo");
        }

        bool IsPlayerLookingAtMe()
        {
            Vector3 directionToEnemy = (transform.position - player.position).normalized;
            float angle = Vector3.Angle(player.forward, directionToEnemy);
            return angle < fieldOfView;
        }

        void OnDrawGizmosSelected()
        {
            if (player == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.yellow;
            Vector3 left = Quaternion.Euler(0, -fieldOfView, 0) * player.forward;
            Vector3 right = Quaternion.Euler(0, fieldOfView, 0) * player.forward;

            Gizmos.DrawRay(player.position, left * 5f);
            Gizmos.DrawRay(player.position, right * 5f);
        }
}
