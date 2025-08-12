using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 20f;
    public float staminaRecoveryRate = 15f;
    public float recoveryDelay = 1f;

    private bool isDraining = false;
    private float recoveryTimer;

    public Slider staminaBar;

    private void Awake()
    {
        currentStamina = maxStamina;


    }

    private void Update()
    {
        if (isDraining)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            recoveryTimer = 0f;
        }
        else
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }
        }




    }

    public void SetDraining(bool state)
    {
        isDraining = state;
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }

    public float GetNormalizedStamina()
    {
        return currentStamina / maxStamina;
    }
}
