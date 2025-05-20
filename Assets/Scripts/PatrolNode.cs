using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolNode : Node
{
    private NavMeshAgent agent;
    private Transform[] waypoints;
    private GuardAI ai;
    private int currentIndex = -1;
    private bool destinationSet = false;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private float defaultWaitTime = 3f; // Fallback wait time if WaypointConfig fails

    public PatrolNode(NavMeshAgent agent, Transform[] waypoints, GuardAI ai)
    {
        this.agent = agent;
        this.waypoints = waypoints;
        this.ai = ai;
        Debug.Log($"PatrolNode initialized with {waypoints.Length} waypoints");
        foreach (var wp in waypoints)
        {
            Debug.Log($"Waypoint {wp.name} has tag: {wp.tag}, WaitTime: {WaypointConfig.GetWaitTime(wp)}");
        }
    }

    public override NodeState Evaluate()
    {
        Debug.Log($"PatrolNode Evaluate called, isWaiting: {isWaiting}, destinationSet: {destinationSet}, currentIndex: {currentIndex}");

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints defined!");
            return NodeState.Failure;
        }

        // Log NavMeshAgent state
        Debug.Log($"Agent state: isStopped: {agent.isStopped}, pathPending: {agent.pathPending}, remainingDistance: {agent.remainingDistance}, stoppingDistance: {agent.stoppingDistance}");

        // If waiting at a waypoint, count down the timer
        if (isWaiting)
        {
            float waypointWaitTime = WaypointConfig.GetWaitTime(waypoints[currentIndex]);
            waitTimer += Time.deltaTime;
            if (waitTimer >= waypointWaitTime)
            {
                Debug.Log($"Finished waiting at waypoint {waypoints[currentIndex].name} for {waypointWaitTime} seconds");
                isWaiting = false;
                destinationSet = false; // Allow selecting a new waypoint
                waitTimer = 0f;
            }
            return NodeState.Running;
        }

        // Check if the agent has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            float waypointWaitTime = WaypointConfig.GetWaitTime(waypoints[currentIndex]);
            Debug.Log($"Reached waypoint {waypoints[currentIndex].name}, waiting for {waypointWaitTime} seconds");
            isWaiting = true;
            waitTimer = 0f;
            return NodeState.Running;
        }

        // Set a new destination if none is set
        if (!destinationSet)
        {
            // Get a list of valid waypoint indices
            var validIndices = new List<int>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                bool isBasement = waypoints[i].CompareTag("Basement");
                if (ai.isAlerted || !isBasement)
                {
                    validIndices.Add(i);
                }
            }

            // If no valid waypoints are available, fail
            if (validIndices.Count == 0)
            {
                Debug.LogWarning("No valid waypoints available!");
                return NodeState.Failure;
            }

            // Choose a random valid waypoint, avoiding the current one if possible
            int newIndex;
            int attempts = 0;
            const int maxAttempts = 10;
            do
            {
                newIndex = validIndices[Random.Range(0, validIndices.Count)];
                attempts++;
            } while (newIndex == currentIndex && validIndices.Count > 1 && attempts < maxAttempts);

            currentIndex = newIndex;
            agent.SetDestination(waypoints[currentIndex].position);
            destinationSet = true;
            Debug.Log($"Moving to waypoint {waypoints[currentIndex].name}, Basement: {waypoints[currentIndex].CompareTag("Basement")}, Alerted: {ai.isAlerted}");
        }

        // Ensure agent's destination matches the intended waypoint
        if (destinationSet && Vector3.Distance(agent.destination, waypoints[currentIndex].position) > 0.1f)
        {
            Debug.LogWarning($"Agent destination mismatch! Expected: {waypoints[currentIndex].name} at {waypoints[currentIndex].position}, Actual: {agent.destination}. Resetting.");
            agent.SetDestination(waypoints[currentIndex].position);
        }

        return NodeState.Running;
    }

    public void OnDrawGizmos()
    {
        if (waypoints != null && currentIndex >= 0 && currentIndex < waypoints.Length)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoints[currentIndex].position, 0.5f);
        }
    }
}