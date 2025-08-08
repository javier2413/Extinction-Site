using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health = 100;

    private bool isEnemyDead = false;
    private EnemyAnimations enemyAnimations;
    private EnemyController enemyController;

    private void Start()
    {
        enemyAnimations = GetComponent<EnemyAnimations>();
        enemyController = GetComponent<EnemyController>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isEnemyDead) return;

        health -= damageAmount;

        ReactToDamage();
        
    }

    private void ReactToDamage()
    {
        enemyAnimations.ReactToLight();
    }

    public bool IsEnemyDead()
    {
        return isEnemyDead;
    }
}