using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private float yaw = 0f;   // Y-axis rotation (left/right)
    private float pitch = 0f; // X-axis rotation (up/down)

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            yaw -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            yaw += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.UpArrow))
            pitch -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            pitch += rotationSpeed * Time.deltaTime;

        // Clamp pitch to prevent flipping
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}






