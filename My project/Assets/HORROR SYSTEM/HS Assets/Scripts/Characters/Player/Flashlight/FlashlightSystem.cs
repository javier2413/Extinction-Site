using UnityEngine;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Light Settings")]
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode flashKey = KeyCode.Space;
    public KeyCode rechargeKey = KeyCode.R;
    public float flashDuration = 0.5f;
    public float flashCooldown = 3f;

    [Header("Battery Settings")]
    public float maxBattery = 10f;
    public float currentBattery;
    public float intensityDrainRate = 0.5f;
    public float minIntensity = 0.2f;
    public float maxIntensity = 3f;
    public float flashIntensity = 5f;

    [Header("Stun Settings")]
    public float stunDuration = 10f;

    private bool isFlashlightOn = false;
    private bool isFlashing = false;
    private bool flashOnCooldown = false;

    void Start()
    {
        if (!spotLight)
        {
            Debug.LogError("Flashlight Light not assigned.");
            enabled = false;
            return;
        }

        currentBattery = maxBattery;
        spotLight.enabled = false;
        spotLight.intensity = maxIntensity;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleFlashlight(!isFlashlightOn);

        if (Input.GetKeyDown(rechargeKey))
            RechargeBattery();

        if (Input.GetKeyUp(flashKey) && !isFlashing && !flashOnCooldown && isFlashlightOn)
            StartCoroutine(FlashCoroutine());

        if (isFlashlightOn && !isFlashing)
        {
            currentBattery -= intensityDrainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);
            spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, currentBattery / maxBattery);

            if (currentBattery <= 0f)
                ToggleFlashlight(false);
        }
    }

    public void ToggleFlashlight(bool turnOn)
    {
        isFlashlightOn = turnOn;
        spotLight.enabled = turnOn;
        spotLight.intensity = turnOn ? Mathf.Lerp(minIntensity, maxIntensity, currentBattery / maxBattery) : 0f;
    }

    public bool IsOn() => isFlashlightOn;

    public void RechargeBattery()
    {
        currentBattery = maxBattery;
        Debug.Log("Battery recharged to max!");
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        flashOnCooldown = true;

        spotLight.intensity = flashIntensity;

        StunRaptorsInLight();

        yield return new WaitForSeconds(flashDuration);

        spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, currentBattery / maxBattery);

        isFlashing = false;

        yield return new WaitForSeconds(flashCooldown);
        flashOnCooldown = false;
    }

    private void StunRaptorsInLight()
    {
        float maxDistance = spotLight.range;
        float halfAngle = spotLight.spotAngle / 2f;

        Collider[] hits = Physics.OverlapSphere(spotLight.transform.position, maxDistance);

        foreach (var hit in hits)
        {
            Raptor raptor = hit.GetComponent<Raptor>();
            if (raptor != null)
            {
                Vector3 dirToRaptor = (raptor.transform.position - spotLight.transform.position).normalized;
                float angle = Vector3.Angle(spotLight.transform.forward, dirToRaptor);
                float distance = Vector3.Distance(spotLight.transform.position, raptor.transform.position);

                if (angle <= halfAngle && distance <= maxDistance && Input.GetKey(KeyCode.Space))
                {
                    raptor.StunFromFlashlight(spotLight, stunDuration);
                }
            }
        }
    }
}



