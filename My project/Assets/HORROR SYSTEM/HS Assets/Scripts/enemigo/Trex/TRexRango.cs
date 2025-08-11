using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TRexRango : MonoBehaviour
{
    public NavMeshAgent Trex;
    public float velocidad;
    public bool Persiguiendo;
    public float Rango;
    float Distacia;

    public Transform objetivo;

    [Header("animaciones")]
    public Animation Animation;
    public string NombreAnimacionCaminata;
    public string NombreAnimacionQuieto;

    private void Update()
    {
        Distacia = Vector3.Distance(Trex.transform.position, objetivo.position);

        if(Distacia < Rango)
        {
            Persiguiendo = true;
        } 
        else if(Distacia > Rango + 3)
        {
            Persiguiendo = false;
        }
        if (Persiguiendo == false)
        {
            Trex.speed = 0;
            Animation.CrossFade(NombreAnimacionQuieto);
        }
        else if(Persiguiendo == true)
        {
            Trex.speed = velocidad;
            Animation.CrossFade(NombreAnimacionCaminata);

            Trex.SetDestination(objetivo.position);
        } 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Trex.transform.position,Rango);
    }
}
