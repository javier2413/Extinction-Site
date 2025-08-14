using UnityEngine;
using System.Collections;

public class EnemyAnimations : MonoBehaviour
{
    private Animator enemyAnimator;

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    // Play Walk animation and stop Run/Stun
    public void Walk()
    {
        enemyAnimator.SetBool("Walk", true);
        enemyAnimator.SetBool("Run", false);
        enemyAnimator.SetBool("IsStunned", false);
    }

    // Stop walking
    public void StopWalking()
    {
        enemyAnimator.SetBool("Walk", false);
    }

    // Play Run animation and stop Walk/Stun
    public void Run()
    {
        enemyAnimator.SetBool("Run", true);
        enemyAnimator.SetBool("Walk", false);
        enemyAnimator.SetBool("IsStunned", false);
    }

    // Stop running
    public void StopRunning()
    {
        enemyAnimator.SetBool("Run", false);
    }

    // Play stun animation and stop Walk/Run
    public void ReactToLight()
    {
        enemyAnimator.SetBool("IsStunned", true);
        enemyAnimator.SetBool("Walk", false);
        enemyAnimator.SetBool("Run", false);

        StartCoroutine(ResetStun());
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(5f); // Duration matches your raptor stun
        enemyAnimator.SetBool("IsStunned", false);
        // Optionally, return to Walk after stun
        Walk();
    }
}

