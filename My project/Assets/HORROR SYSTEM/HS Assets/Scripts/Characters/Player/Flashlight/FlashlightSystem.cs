using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Light")]
    public Light spotLight = null;

    [Header("Intensity Settings")]
    public int maxIntensity;
    public float currentIntensity;
    public float intensityChange;

    [Header("Audio Settings")]
    public string reloadFlashlightSound;

    private void Start()
    {
        currentIntensity = maxIntensity;
        spotLight.intensity = currentIntensity;
        spotLight.enabled = false;
    }

    private void Update()
    {
        BatteryChange();
    
        if (spotLight.enabled)
        {
            CheckForRaptorsInLight();
        }
    }

    void CheckForRaptorsInLight()
    {
        float maxDistance = spotLight.range;
        float spotAngle = spotLight.spotAngle / 2f; // half angle

        Collider[] hits = Physics.OverlapSphere(spotLight.transform.position, maxDistance);

        foreach (Collider hit in hits)
        {
            Raptor raptor = hit.GetComponent<Raptor>();
            if (raptor != null)
            {
                Vector3 directionToRaptor = (raptor.transform.position - spotLight.transform.position).normalized;
                float angleToRaptor = Vector3.Angle(spotLight.transform.forward, directionToRaptor);

                if (angleToRaptor <= spotAngle)
                {
                    // Raptor is inside flashlight cone
                    raptor.Stun(5f); // stun you gotta survive man
                }
            }
        }
    }

    public void SetMaxIntensity()
    {
        currentIntensity = maxIntensity;
        AudioManager.instance.Play(reloadFlashlightSound);
    }

    public void ToggleFlashlight(bool isFlashlightOn)
    {
        if (spotLight.enabled != isFlashlightOn)
        {
            spotLight.enabled = isFlashlightOn;

            if (isFlashlightOn)
            {
                BatteryChange();
            }
        }
    }

    private void BatteryChange()
    {
        currentIntensity -= intensityChange * Time.deltaTime;
        spotLight.intensity = Mathf.Lerp(0f, maxIntensity, currentIntensity / maxIntensity);

        if (currentIntensity < 0)
        {
            currentIntensity = 0;
        }
    }
}