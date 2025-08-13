using UnityEngine;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Light Settings")]
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;      // Key to toggle flashlight
    public KeyCode flashKey = KeyCode.Space;       // Key to trigger flash (same as toggle for hold/release)
    public KeyCode rechargeKey = KeyCode.R;    // Key to manually recharge battery
    public float maxIntensity = 3f;
    public float minIntensity = 0.2f;
    public float flashIntensity = 5f;
    public float intensityDrainRate = 0.5f;
    public float rechargeRate = 1f;
    public float flashDuration = 0.5f;
    public float flashCooldown = 3f;

    [Header("Animation Settings")]
    public PlayerAnimations playerAnimations;

    [Header("Battery Settings")]
    public float currentBattery;
    public float maxBattery = 10f;

    [Header("Enemy Detection")]
    public float stunDuration = 5f;

    [Header("Audio Settings")]
    public string reloadSound;

    private bool isFlashlightOn = false;
    private bool isFlashing = false;
    private bool flashOnCooldown = false;

    private void Start()
    {
        if (spotLight == null)
        {
            Debug.LogError("No Light assigned to flashlight system.");
            enabled = false;
            return;
        }

        if (playerAnimations == null)
            playerAnimations = GetComponent<PlayerAnimations>();

        currentBattery = maxBattery;
        spotLight.enabled = false;
        spotLight.intensity = maxIntensity;

        playerAnimations?.SetFlashlightIdle(false);
    }


    private void Update()
    {

        // Manual recharge key (optional)
        if (Input.GetKeyDown(rechargeKey))
        {
            RechargeBattery();
        }

        if (Input.GetKeyUp(flashKey) && !isFlashing && !flashOnCooldown)
        {
            StartCoroutine(FlashCoroutine());
        }

        if (isFlashlightOn)
        {
            // Drain battery continuously if not flashing
            if (!isFlashing)
            {
                currentBattery -= intensityDrainRate * Time.deltaTime;
                currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

                // Update intensity based on battery
                float batteryRatio = currentBattery / maxBattery;
                spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, batteryRatio);

                // Auto turn off if battery empty
                if (currentBattery <= 0)
                {
                    ToggleFlashlight(false);
                    return;
                }
            }

            CheckForRaptorsInLight();

            // Trigger flash on key release if possible
            if (Input.GetKeyUp(flashKey) && !isFlashing && !flashOnCooldown)
            {
                StartCoroutine(FlashCoroutine());
            }
        }
    }

    public bool IsOn()
    {
        return isFlashlightOn;
    }

    public void ToggleFlashlight(bool turnOn)
    {
        isFlashlightOn = turnOn;
        spotLight.enabled = turnOn;

        if (turnOn)
        {
            float batteryRatio = currentBattery / maxBattery;
            spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, batteryRatio);
        }
        else
        {
            spotLight.intensity = 0f;
        }

        // Sync animation state
        playerAnimations?.SetFlashlightIdle(turnOn);
    }


    public void RechargeIntensity(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);

        float batteryRatio = currentBattery / maxBattery;
        spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, batteryRatio);
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        flashOnCooldown = true;

        spotLight.intensity = flashIntensity;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;
        float startIntensity = spotLight.intensity;
        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, currentBattery / maxBattery);

        // Smoothly return to battery intensity
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * rechargeRate;
            spotLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed);
            yield return null;
        }

        isFlashing = false;

        // Flash cooldown to prevent spamming
        yield return new WaitForSeconds(flashCooldown);
        flashOnCooldown = false;
    }

    private void CheckForRaptorsInLight()
    {
        float maxDistance = spotLight.range;
        float halfSpotAngle = spotLight.spotAngle / 2f;

        Collider[] hits = Physics.OverlapSphere(spotLight.transform.position, maxDistance);

        foreach (Collider hit in hits)
        {
            Raptor raptor = hit.GetComponent<Raptor>();
            if (raptor != null)
            {
                Vector3 dirToRaptor = (raptor.transform.position - spotLight.transform.position).normalized;
                float angle = Vector3.Angle(spotLight.transform.forward, dirToRaptor);

                if (angle <= halfSpotAngle)
                {
                    raptor.Stun(stunDuration);
                }
            }
        }
    }

    public void RechargeBattery()
    {
        currentBattery = maxBattery;

        if (!string.IsNullOrEmpty(reloadSound) && AudioManager.instance != null)
        {
            AudioManager.instance.Play(reloadSound);
        }
    }
}