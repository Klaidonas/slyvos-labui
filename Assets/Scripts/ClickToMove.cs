using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;  // Reference to Animator
    private int groundLayerMask;

    private float defaultSpeed = 3.5f;
    private float runSpeed = 7.0f;
    private float doubleClickTime = 0.3f;
    private float lastClickTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();  // Get the Animator component
        if (agent == null)
        {
            Debug.LogError("❌ NavMeshAgent component is missing on player!");
        }
        if (animator == null)
        {
            Debug.LogError("❌ Animator component is missing on player!");
        }

        groundLayerMask = LayerMask.GetMask("Ground");
        if (groundLayerMask == 0)
        {
            Debug.LogError("❌ 'Ground' layer not found!");
        }

        agent.speed = defaultSpeed;
        agent.updateRotation = true;  // Enable automatic rotation by default
    }

    void Update()
    {
        // Handle movement and animation state
        HandleMovement();

        // Check for plank crossing
        CheckPlankCrossing();
    }

    void HandleMovement()
    {
        // When the character is moving
        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetFloat("speed", agent.velocity.magnitude);  // Set Speed parameter in Animator
            agent.updateRotation = true;  // Enable rotation when moving
        }
        else  // If the character is not moving
        {
            animator.SetFloat("speed", 0);  // Set Speed parameter to 0 for idle state
            agent.updateRotation = false;  // Disable rotation when idle

            // If character has reached destination, stop movement completely
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                // Stop the sneaking and running animations
                animator.SetBool("isSneaking", false);  // Lowercase "isSneaking"
                animator.SetBool("isRunning", false);   // Lowercase "isRunning"
                animator.SetFloat("speed", 0);          // Set speed to 0 for Idle animation

                // Stop the NavMeshAgent's movement and rotation
                agent.isStopped = true;
                agent.velocity = Vector3.zero; // Ensure no movement

                // Reset booleans to uncheck the boxes
                animator.SetBool("isSneaking", false);
                animator.SetBool("isRunning", false);
            }
        }

        if (Input.GetMouseButtonDown(0)) // Detect left mouse button click
        {
            // Reset all boolean flags for previous animation states
            animator.SetBool("isSneaking", false);  // Lowercase "isSneaking"
            animator.SetBool("isRunning", false);   // Lowercase "isRunning"

            if (Time.time - lastClickTime <= doubleClickTime)
            {
                // Double click detected, set to running
                animator.SetBool("isRunning", true);  // Lowercase "isRunning"
                agent.speed = runSpeed;
                Debug.Log("🏃‍♂️ Running at 2x speed!");
            }
            else
            {
                // Single click detected, set to sneaking
                animator.SetBool("isSneaking", true);  // Lowercase "isSneaking"
                agent.speed = defaultSpeed;
            }

            lastClickTime = Time.time;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            Vector3 targetPosition;
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                Debug.Log("✅ Clicked on: " + hit.point + " | Object: " + hit.collider.name);
                if (hit.collider.CompareTag("Stairs"))
                {
                    agent.speed = 2.0f;
                }
                targetPosition = hit.point;
            }
            else
            {
                Debug.Log("⚠️ No direct hit, finding nearest NavMesh point...");
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 10f; // Adjust based on your scene’s depth
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);

                if (NavMesh.SamplePosition(worldPoint, out NavMeshHit navHit, 15f, NavMesh.AllAreas))
                {
                    targetPosition = navHit.position;
                    Debug.Log("✅ Nearest valid NavMesh point: " + targetPosition);
                }
                else
                {
                    Debug.Log("❌ No valid NavMesh point found.");
                    return;
                }
            }

            agent.SetDestination(targetPosition);
            Debug.DrawLine(targetPosition, targetPosition + Vector3.up * 2f, Color.green, 2f);

    }

}

