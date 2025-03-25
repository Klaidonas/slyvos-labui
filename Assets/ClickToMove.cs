using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private int groundLayerMask; // Layer mask for the "Ground" layer

    private float defaultSpeed = 3.5f; // Default movement speed
    private float runSpeed = 7.0f; // Running speed (2x default speed)
    private float doubleClickTime = 0.3f; // Time window for double-click detection
    private float lastClickTime = 0f; // Time of the last click

    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();

        // Check if the NavMeshAgent component is missing
        if (agent == null)
        {
            Debug.LogError("❌ NavMeshAgent component is missing on player!");
        }

        // Get the layer mask for the "Ground" layer
        groundLayerMask = LayerMask.GetMask("Ground");

        // Check if the "Ground" layer exists
        if (groundLayerMask == 0)
        {
            Debug.LogError("❌ 'Ground' layer not found! Make sure it's spelled correctly in the Layer Manager.");
        }

        // Set the default speed
        agent.speed = defaultSpeed;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to move
        {
            // Check for double-click
            if (Time.time - lastClickTime <= doubleClickTime)
            {
                // Double-click detected: Set running speed
                agent.speed = runSpeed;
                Debug.Log("🏃‍♂️ Running at 2x speed!");
            }
            else
            {
                // Single-click: Reset to default speed
                agent.speed = defaultSpeed;
            }

            // Update the last click time
            lastClickTime = Time.time;

            // Handle movement
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Debug: Draw the ray in the Scene view
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                Debug.Log("✅ Clicked on: " + hit.point + " | Object: " + hit.collider.name);

                // Check if the clicked object is stairs
                if (hit.collider.CompareTag("Stairs"))
                {
                    // Adjust movement speed for stairs
                    agent.speed = 2.0f; // Slower speed for stairs
                }

                // Set the NavMeshAgent's destination to the hit point
                agent.SetDestination(hit.point);
            }
            else
            {
                Debug.Log("⚠️ No valid surface clicked.");
            }
        }
    }
}