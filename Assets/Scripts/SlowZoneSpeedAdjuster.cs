using UnityEngine;
using UnityEngine.AI;

public class SlowZoneSpeedAdjuster : MonoBehaviour
{
    private NavMeshAgent agent;
    private float originalSpeed;
    private readonly float speedReductionFactor = 0.7f; // 30% speed reduction
    private bool isSlowed = false;

    // Set this to the index of your "SlowZone" area (check Navigation > Areas tab)
    private readonly int slowZoneAreaIndex = 3; // Example: Walkable=0, Not Walkable=1, Jump=2, SlowZone=3

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            originalSpeed = agent.speed;
            Debug.Log($"[{Time.time}] SlowZoneSpeedAdjuster initialized for {gameObject.name}, Original Speed: {originalSpeed}");
        }
        else
        {
            Debug.LogWarning($"[{Time.time}] No NavMeshAgent found on {gameObject.name}. Disabling script.");
            enabled = false;
        }
    }

    void Update()
    {
        // Sample the NavMesh at the agent's position
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            // Check if the agent is in the "SlowZone" area
            if (hit.mask == 1 << slowZoneAreaIndex)
            {
                if (!isSlowed)
                {
                    agent.speed = originalSpeed * speedReductionFactor;
                    isSlowed = true;
                    Debug.Log($"[{Time.time}] Slowed down {gameObject.name} to speed {agent.speed}");
                }
            }
            else
            {
                if (isSlowed)
                {
                    agent.speed = originalSpeed;
                    isSlowed = false;
                    Debug.Log($"[{Time.time}] Restored speed for {gameObject.name} to {agent.speed}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"[{Time.time}] NavMesh.SamplePosition failed for {gameObject.name} at position {transform.position}");
        }
    }

    void OnDestroy()
    {
        if (agent != null)
        {
            agent.speed = originalSpeed;
            Debug.Log($"[{Time.time}] Restored speed for {gameObject.name} on destroy to {originalSpeed}");
        }
    }
}