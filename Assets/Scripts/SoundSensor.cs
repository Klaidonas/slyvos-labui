using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SoundSensor : MonoBehaviour
{
    [Header("Sound Radius Settings")]
    public float idleRadius = 0.3f;     // When standing still
    public float walkRadius = 3.5f;     // Normal walking speed
    public float runRadius = 7f;        // Double-click running speed

    [Header("Speed Thresholds")]
    [SerializeField] private float walkSpeedThreshold = 2f; // Speed threshold for walking
    [SerializeField] private float runSpeedThreshold = 5f;  // Speed threshold for running

    [Header("Debug")]
    public bool showVisualization = true;
    public Color gizmoColor = new Color(1, 0.5f, 0, 0.5f); // Orange, increased opacity

    private SphereCollider soundTrigger;
    private UnityEngine.AI.NavMeshAgent agent;
    private float targetRadius;

    void Start()
    {
        // Ensure SphereCollider exists
        soundTrigger = GetComponent<SphereCollider>();
        if (soundTrigger == null)
        {
            Debug.LogError($"SoundSensor on {gameObject.name}: SphereCollider missing! Adding one...");
            soundTrigger = gameObject.AddComponent<SphereCollider>();
        }
        soundTrigger.isTrigger = true;
        Debug.Log($"SoundSensor on {gameObject.name}: SphereCollider initialized, isTrigger = {soundTrigger.isTrigger}, enabled = {soundTrigger.enabled}, radius = {soundTrigger.radius}");

        // Find NavMeshAgent
        agent = GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"SoundSensor on {gameObject.name}: NavMeshAgent missing on parent!");
        }
        else
        {
            Debug.Log($"SoundSensor on {gameObject.name}: Found NavMeshAgent on {agent.gameObject.name}");
        }
    }

    void Update()
    {
        if (agent == null)
        {
            Debug.LogWarning($"SoundSensor on {gameObject.name}: Cannot update - NavMeshAgent is null!");
            return;
        }

        if (soundTrigger == null || !soundTrigger.enabled)
        {
            Debug.LogWarning($"SoundSensor on {gameObject.name}: Cannot update - SphereCollider is null or disabled!");
            return;
        }

        // Calculate velocity and movement state
        float velocity = agent.velocity.magnitude;
        bool isMoving = velocity > 0.1f;

        // Set target radius
        if (!isMoving)
        {
            targetRadius = idleRadius;
        }
        else
        {
            targetRadius = velocity >= runSpeedThreshold ? runRadius : walkRadius;
        }

        // Smoothly adjust radius
        soundTrigger.radius = Mathf.Lerp(soundTrigger.radius, targetRadius, Time.deltaTime * 8f);
        Debug.Log($"SoundSensor on {gameObject.name}: velocity = {velocity}, targetRadius = {targetRadius}, current radius = {soundTrigger.radius}");
    }

    void OnDrawGizmos()
    {
        if (!showVisualization || !Application.isPlaying)
        {
            return;
        }

        if (soundTrigger == null)
        {
            soundTrigger = GetComponent<SphereCollider>();
            if (soundTrigger == null)
            {
                return;
            }
        }

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, soundTrigger.radius);
    }
}