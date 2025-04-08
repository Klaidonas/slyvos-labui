using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private int groundLayerMask;

    private float defaultSpeed = 3.5f;
    private float runSpeed = 7.0f;
    private float doubleClickTime = 0.3f;
    private float lastClickTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("❌ NavMeshAgent component is missing on player!");
        }

        groundLayerMask = LayerMask.GetMask("Ground");
        if (groundLayerMask == 0)
        {
            Debug.LogError("❌ 'Ground' layer not found!");
        }

        agent.speed = defaultSpeed;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime <= doubleClickTime)
            {
                agent.speed = runSpeed;
                Debug.Log("🏃‍♂️ Running at 2x speed!");
            }
            else
            {
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

        CheckPlankCrossing();
    }

    private void CheckPlankCrossing()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1f, groundLayerMask))
        {
            Plank plank = hit.collider.GetComponentInParent<Plank>();
            if (plank != null)
            {
                plank.CrossPlank();
            }
        }
    }
}