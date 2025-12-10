using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  

    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;

    public float sensitivity = 2f;
    public Vector2 pitchLimits = new Vector2(-30f, 60f); 

    private float yaw = 0f;
    private float pitch = 20f;
    private PauseMenu pauseMenu; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        // Don't move camera when paused
        if (pauseMenu != null && pauseMenu.IsPaused())
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);

        Vector3 forward = transform.forward;
        forward.y = 0; 
        target.rotation = Quaternion.LookRotation(forward);
    }
}
