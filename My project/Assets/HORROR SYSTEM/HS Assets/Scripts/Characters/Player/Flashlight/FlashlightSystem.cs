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
        maxIntensity = 3;
        if (spotLight == null)
        {
            Debug.LogError("spotLight is NOT assigned in inspector!");
        }
        else
        {
            Debug.Log("spotLight assigned: " + spotLight.name);
            spotLight.intensity = currentIntensity;
            spotLight.enabled = false;
        }
    }
    private void Update()
    {
        //BatteryChange();

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight(!spotLight.enabled);
        }

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
                    raptor.Stun(5f); // Stun the raptor for 5 seconds
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
        Debug.Log("ToggleFlashlight called. Turning flashlight " + (isFlashlightOn ? "ON" : "OFF"));
        spotLight.enabled = isFlashlightOn;

        if (isFlashlightOn)
        {
            BatteryChange();
            spotLight.intensity = maxIntensity;  // reset intensity immediately on toggle on
        }
        else
        {
            spotLight.intensity = 0f;
        }
    }

    private void BatteryChange()
    {
        currentIntensity -= intensityChange * Time.deltaTime;
        currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);

        spotLight.intensity = Mathf.Lerp(0f, maxIntensity, currentIntensity / maxIntensity);

        if (currentIntensity <= 0)
        {
            spotLight.enabled = false;
        }
    }

}