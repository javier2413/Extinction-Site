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

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void Heal(int value)
    {
        if (isPlayerDead) return;

        currentHealth += value;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();

        if (!string.IsNullOrEmpty(treatmentSound))
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
            Die();
    }

    private void Die()
    {
        isPlayerDead = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Opcional: si quieres reiniciar la escena
        if (!string.IsNullOrEmpty(Nivel))
            SceneManager.LoadScene(Nivel);
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

    private void UpdateHealthUI()
    {
        int healthPercentage = Mathf.RoundToInt((float)currentHealth / maxHealth * 100);
        healthText.text = healthPercentage + "%";

        float healthRatio = (float)currentHealth / maxHealth;
        if (healthRatio > 0.5f)
            healthImage.color = Color.Lerp(healthColorMiddle, healthColorMax, (healthRatio - 0.5f) * 2f);
        else if (healthRatio > 0.25f)
            healthImage.color = Color.Lerp(healthColorMin, healthColorMiddle, (healthRatio - 0.25f) * 4f);
        else
            healthImage.color = healthColorMin;

        healthText.color = healthImage.color;
    }

    // Método público para que otros scripts puedan chequear si el jugador está muerto
    public bool IsPlayerDead()
    {
        return isPlayerDead;
    }
}


