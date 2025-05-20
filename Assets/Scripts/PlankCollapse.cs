using UnityEngine;

public class PlankCollapse : MonoBehaviour
{
    [SerializeField] private int maxPasses = 4;
    private int currentPasses = 0;
    private bool hasCollapsed = false;
    private Rigidbody rb;

    void Awake()
    {
        Debug.Log($"[{Time.time}] Awake called for {gameObject.name} at {transform.position}, active: {gameObject.activeInHierarchy}");
    }

    void Start()
    {
        Debug.Log($"[{Time.time}] Start called for {gameObject.name} at {transform.position}, enabled: {enabled}");
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            Debug.Log($"[{Time.time}] Rigidbody found, gravity off, kinematic on for {gameObject.name}");
        }
        else
        {
            Debug.Log($"[{Time.time}] No Rigidbody on {gameObject.name}");
        }
        Debug.Log($"[{Time.time}] Plank {gameObject.name} initialized with max passes: {maxPasses}");
    }

    void OnEnable()
    {
        Debug.Log($"[{Time.time}] OnEnable called for {gameObject.name}");
    }

    void Update()
    {
        if (!hasCollapsed)
        {
            Debug.Log($"[{Time.time}] Update running for {gameObject.name}, passes: {currentPasses}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{Time.time}] OnTriggerEnter called for {gameObject.name}, triggered by {other.gameObject.name}, tag: {other.tag}, collider: {other}, active: {gameObject.activeInHierarchy}");
        if (hasCollapsed) return;

        if (other.CompareTag("Player"))
        {
            currentPasses++;
            Debug.Log($"[{Time.time}] Plank {gameObject.name} pass #{currentPasses}/{maxPasses} by {other.gameObject.name}");
            if (currentPasses >= maxPasses)
            {
                Collapse();
            }
        }
    }

    void Collapse()
    {
        hasCollapsed = true;
        Debug.Log($"[{Time.time}] Plank {gameObject.name} is collapsing!");
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            Debug.Log($"[{Time.time}] Gravity enabled, kinematic off for {gameObject.name}");
        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log($"[{Time.time}] Plank {gameObject.name} deactivated");
        }
    }
}