using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    // AI Components
    private NavMeshAgent agent;
    private Transform player;

    // Waypoints for roaming
    [SerializeField] private Transform waypointsParent; // Assign the "Waypoints" GameObject in the Inspector
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    // AI States
    private enum State { Roaming, Investigating, Chasing }
    private State currentState = State.Roaming;

    // Detection Parameters
    [Header("Detection Settings")]
    [SerializeField] private float proximityDetectionRange = 3f; // Range to detect player proximity
    [SerializeField] private float fieldOfViewAngle = 60f; // FOV angle for line of sight (degrees)
    [SerializeField] private float viewDistance = 15f; // Max distance for line of sight
    [SerializeField] private LayerMask obstacleMask; // Layers that block line of sight (e.g., walls)

    // Investigation Parameters
    private Vector3 lastHeardPosition;
    private float investigationTimer = 0f;
    [SerializeField] private float investigationDuration = 5f; // How long to investigate a sound

    // Waiting at Waypoints
    [Header("Roaming Settings")]
    [SerializeField] private float waitTimeAtWaypoint = 2f; // Time to wait at each waypoint (in seconds)
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component missing on AI_Agent!");
            return;
        }

        // Restrict AI to "Walkable" area
        int walkableAreaMask = 1 << NavMesh.GetAreaFromName("Walkable");
        agent.areaMask = walkableAreaMask;
        Debug.Log($"AIController: Set areaMask to {agent.areaMask} (Walkable only), Speed: {agent.speed}, Stopping Distance: {agent.stoppingDistance}");

        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
            return;
        }
        Debug.Log($"AIController: Player found: {player.name} at {player.position}");

        // Populate waypoints
        if (waypointsParent == null || waypointsParent.childCount == 0)
        {
            Debug.LogError("WaypointsParent not assigned or has no child waypoints in AIController!");
            return;
        }

        waypoints = new Transform[waypointsParent.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = waypointsParent.GetChild(i);
            Debug.Log($"AIController: Waypoint {i}: {waypoints[i].name} at {waypoints[i].position}");
        }

        // Start roaming
        if (waypoints.Length > 0)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            Debug.Log($"AIController: Starting roaming at waypoint {currentWaypointIndex}: {waypoints[currentWaypointIndex].name} at {waypoints[currentWaypointIndex].position}");
        }
    }

    void Update()
    {
        if (agent == null || waypoints == null || waypoints.Length == 0 || player == null)
        {
            Debug.LogWarning("AIController: Cannot update due to missing components or waypoints!");
            return;
        }

        // State machine
        switch (currentState)
        {
            case State.Roaming:
                Roam();
                break;
            case State.Investigating:
                Investigate();
                break;
            case State.Chasing:
                Chase();
                break;
        }

        // Check for player detection
        DetectPlayer();
    }

    void Roam()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                Debug.Log($"AIController: Moving to waypoint {currentWaypointIndex}: {waypoints[currentWaypointIndex].name} at {waypoints[currentWaypointIndex].position}");
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = 0f;
            Debug.Log($"AIController: Reached waypoint {currentWaypointIndex}: {waypoints[currentWaypointIndex].name}. Waiting for {waitTimeAtWaypoint}s");
        }
        else
        {
            Debug.Log($"AIController: Moving to waypoint {currentWaypointIndex}, remaining distance: {agent.remainingDistance}");
        }
    }

    void Investigate()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            investigationTimer += Time.deltaTime;
            if (investigationTimer >= investigationDuration)
            {
                currentState = State.Roaming;
                investigationTimer = 0f;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                Debug.Log($"AIController: Investigation complete at {lastHeardPosition}. Returning to waypoint {currentWaypointIndex}");
            }
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= 1f)
        {
            Debug.Log("AIController: AI caught the player! Game Over!");
            // Add game over logic here
        }

        if (!CanSeePlayer() && Vector3.Distance(transform.position, player.position) > 10f)
        {
            currentState = State.Roaming;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            Debug.Log("AIController: Lost sight of player. Returning to roaming.");
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log($"AIController: Distance to player = {distanceToPlayer}, current state = {currentState}");

        // Proximity detection
        if (distanceToPlayer <= proximityDetectionRange)
        {
            currentState = State.Chasing;
            Debug.Log("AIController: Player detected via proximity! Chasing...");
            return;
        }

        // Line of sight detection
        if (CanSeePlayer())
        {
            currentState = State.Chasing;
            Debug.Log("AIController: Player spotted via line of sight! Chasing...");
            return;
        }

        // Sound detection
        SoundSensor soundSensor = player.GetComponentInChildren<SoundSensor>();
        if (soundSensor == null)
        {
            Debug.LogWarning($"AIController: No SoundSensor found on Player ({player.name}) or its children!");
            return;
        }

        SphereCollider soundCollider = soundSensor.GetComponent<SphereCollider>();
        if (soundCollider == null || !soundCollider.enabled)
        {
            Debug.LogError($"AIController: SoundSensor on {soundSensor.gameObject.name} has no enabled SphereCollider!");
            return;
        }

        float soundRadius = soundCollider.radius;
        Debug.Log($"AIController: SoundSensor on {soundSensor.gameObject.name}, soundRadius = {soundRadius}, distanceToPlayer = {distanceToPlayer}");
        if (distanceToPlayer <= soundRadius)
        {
            lastHeardPosition = player.position;
            currentState = State.Investigating;
            agent.SetDestination(lastHeardPosition);
            Debug.Log($"AIController: Heard player at {lastHeardPosition}! Investigating... (Sound radius: {soundRadius})");
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle / 2f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, out hit, viewDistance, obstacleMask))
        {
            if (hit.transform != player)
            {
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityDetectionRange);

        Gizmos.color = Color.blue;
        Vector3 leftFOV = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward * viewDistance;
        Vector3 rightFOV = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward * viewDistance;
        Gizmos.DrawLine(transform.position, transform.position + leftFOV);
        Gizmos.DrawLine(transform.position, transform.position + rightFOV);
    }
}