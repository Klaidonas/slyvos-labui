using UnityEngine;

public class PlankSegment : MonoBehaviour
{
    private Plank plank;

    void Start()
    {
        plank = GetComponentInParent<Plank>();
        if (plank == null)
        {
            Debug.LogError($"❌ PlankSegment on {name} could not find Plank script in parent!");
        }
        else
        {
            Debug.Log($"PlankSegment on {name} found Plank script on {plank.name}");
        }

        Collider[] colliders = GetComponents<Collider>();
        bool hasTrigger = false;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                hasTrigger = true;
                Debug.Log($"PlankSegment on {name} has a trigger collider: {collider}");
            }
        }
        if (!hasTrigger)
        {
            Debug.LogWarning($"⚠️ PlankSegment on {name} has no trigger collider!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{name} OnTriggerEnter: Collided with {other.name} (Tag: {other.tag})");
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{other.name} entered plank segment {name}");
            if (plank != null)
            {
                plank.CrossPlank();
            }
        }
    }
}