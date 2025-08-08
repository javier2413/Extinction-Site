using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAnimations : MonoBehaviour
{
    private Animator enemyAnimator;

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    // Animations
    public void Walk()
    {
        enemyAnimator.SetBool("Walk", true);
    }

    public void StopWalking()
    {
        enemyAnimator.SetBool("Walk", false);
    }

    public void Run()
    {
        enemyAnimator.SetBool("Run", true);
    }

    public void StopRunning()
    {
        enemyAnimator.SetBool("Run", false);
    }

    public void Leap()
    {
        enemyAnimator.SetTrigger("Leap"); //basically so that it only activates once and then doesnt need to loop
    }

    public void Roar()
    {
        enemyAnimator.SetTrigger("Roar"); //same case
    }

    public void ReactToLight()
    {
        enemyAnimator.SetBool("IsStunned", true);
        StartCoroutine(ResetStun());
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(1.5f);
        enemyAnimator.SetBool("IsStunned", false);
    }
}
