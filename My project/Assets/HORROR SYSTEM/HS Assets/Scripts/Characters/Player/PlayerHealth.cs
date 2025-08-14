using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public string Nivel;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health UI")]
    public TMP_Text healthText;
    public Image healthImage;

    [Header("Audio Settings")]
    public string treatmentSound;

    [Header("Health UI Color Settings")]
    public Color healthColorMax = Color.green;
    public Color healthColorMiddle = Color.yellow;
    public Color healthColorMin = Color.red;

    [Header("Blood Splatter")]
    public GameObject bloodSplatterCanvas;
    private Coroutine bloodCoroutine;

    private bool isPlayerDead = false;
    private PlayerAnimations playerAnimations;

    private void Start()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void Heal(int value)
    {
        if (isPlayerDead) return;

        currentHealth += value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    public void SetMaxHealth()
    {
        if (isPlayerDead) return;

        currentHealth = maxHealth;
        UpdateHealthUI();
        AudioManager.instance.Play(treatmentSound);
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerDead) return;

        if (bloodCoroutine != null)
            StopCoroutine(bloodCoroutine);

        bloodCoroutine = StartCoroutine(ShowBloodSplatter());

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
            SceneManager.LoadScene(Nivel);
        }
    }

    private void UpdateHealthUI()
    {
        UpdateHealthText();
        UpdateHealthImageColor();
    }

    private void UpdateHealthText()
    {
        int healthPercentage = Mathf.RoundToInt((float)currentHealth / maxHealth * 100);
        healthText.text = healthPercentage + "%";
    }

    private void UpdateHealthImageColor()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        if (healthRatio > 0.5f)
            healthImage.color = Color.Lerp(healthColorMiddle, healthColorMax, (healthRatio - 0.5f) * 2f);
        else if (healthRatio > 0.25f)
            healthImage.color = Color.Lerp(healthColorMin, healthColorMiddle, (healthRatio - 0.25f) * 4f);
        else
            healthImage.color = healthColorMin;

        healthText.color = healthImage.color;
    }

    private IEnumerator ShowBloodSplatter()
    {
        if (bloodSplatterCanvas != null)
        {
            bloodSplatterCanvas.SetActive(true);
            yield return new WaitForSeconds(3f);
            bloodSplatterCanvas.SetActive(false);
        }
    }

    private void Die()
    {
        isPlayerDead = true;
        playerAnimations?.SetDeath();
        playerAnimations?.ResetRigWeights();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public bool IsPlayerDead()
    {
        return isPlayerDead;
    }
}
