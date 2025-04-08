using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player in the Inspector
    public float followSpeed = 5f;  // How smoothly the camera follows
    public float mouseInfluenceX = 3f; // Side-to-side movement effect
    public float mouseInfluenceY = 1.5f; // Up-down movement effect
    public Vector2 maxMouseOffset = new Vector2(1f, 1f); // Maximum movement limits (X, Y)

    public float zoomSpeed = 5f; // Speed of zooming in/out
    public float minZoom = 5f; // Minimum zoom distance
    public float maxZoom = 15f; // Maximum zoom distance
    public float currentZoom = 10f; // Current zoom distance

    private Vector3 baseOffset; // Base offset from the player (fixed angle)
    private Vector3 mouseOffset; // Mouse influence offset

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow script: No player assigned!");
            return;
        }

        // Initialize the base offset (fixed angle, no rotation)
        baseOffset = transform.position - player.position;
        currentZoom = baseOffset.magnitude; // Initialize currentZoom based on the initial offset
    }

    void Update()
    {
        if (player == null) return;

        // Handle zooming
        HandleZoom();

        // Get mouse position as a percentage of the screen (-1 to 1)
        float mouseX = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float mouseY = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        // Calculate X and Y offsets (mouse influence)
        float horizontalOffset = Mathf.Clamp(mouseX * mouseInfluenceX, -maxMouseOffset.x, maxMouseOffset.x);
        float verticalOffset = Mathf.Clamp(mouseY * mouseInfluenceY, -maxMouseOffset.y, maxMouseOffset.y);

        // Update the mouse offset
        mouseOffset = new Vector3(horizontalOffset, verticalOffset, 0);

        // Calculate the desired position
        Vector3 desiredPosition = player.position + baseOffset.normalized * currentZoom;

        // Add mouse influence relative to the camera's fixed angle
        desiredPosition += transform.right * mouseOffset.x + transform.up * mouseOffset.y;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    void HandleZoom()
    {
        // Get scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Adjust zoom distance based on scroll input
        if (scrollInput != 0)
        {
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); // Clamp zoom distance
        }
    }
}