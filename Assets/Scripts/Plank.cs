using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Plank : MonoBehaviour
{
    private int crossings = 0;
    private int maxCrossings = 5;
    private GameObject[] segmentColliders;
    private NavMeshModifier[] segmentModifiers;
    private Rigidbody[] segmentRigidbodies;
    private bool isBroken = false;

    void Start()
    {
        segmentColliders = new GameObject[2];
        segmentModifiers = new NavMeshModifier[2];
        segmentRigidbodies = new Rigidbody[2];
        if (name == "Plank1")
        {
            Transform plank1_1 = transform.Find("plank 1_1");
            Transform plank1_2 = transform.Find("plank 1_2");
            if (plank1_1 != null && plank1_2 != null)
            {
                segmentColliders[0] = plank1_1.Find("narrow_plank_1")?.gameObject;
                segmentColliders[1] = plank1_2.Find("narrow_plank_2")?.gameObject;
            }
        }
        else if (name == "Plank2")
        {
            Transform plank2_1 = transform.Find("plank 2_1");
            Transform plank2_2 = transform.Find("plank 2_2");
            if (plank2_1 != null && plank2_2 != null)
            {
                segmentColliders[0] = plank2_1.Find("wide_plank_1")?.gameObject;
                segmentColliders[1] = plank2_2.Find("wide_plank_2")?.gameObject;
            }
        }

        if (segmentColliders[0] == null || segmentColliders[1] == null)
        {
            Debug.LogError($"❌ Plank {name} is missing segment GameObjects!");
            return;
        }

        for (int i = 0; i < segmentColliders.Length; i++)
        {
            segmentModifiers[i] = segmentColliders[i].GetComponent<NavMeshModifier>();
            if (segmentModifiers[i] == null)
            {
                segmentModifiers[i] = segmentColliders[i].AddComponent<NavMeshModifier>();
                segmentModifiers[i].overrideArea = true;
                segmentModifiers[i].area = 0; // "Walkable" area (index 0)
                Debug.Log($"Added NavMeshModifier to {segmentColliders[i].name} with area = Walkable");
            }

            segmentRigidbodies[i] = segmentColliders[i].GetComponent<Rigidbody>();
            if (segmentRigidbodies[i] == null)
            {
                segmentRigidbodies[i] = segmentColliders[i].AddComponent<Rigidbody>();
                segmentRigidbodies[i].isKinematic = true;
                segmentRigidbodies[i].useGravity = false;
            }
        }

        foreach (var segment in segmentColliders)
        {
            Debug.Log($"Plank {name} segment: {segment.name}");
        }
    }

    public void CrossPlank()
    {
        Debug.Log($"CrossPlank called. isBroken: {isBroken}, crossings: {crossings}, maxCrossings: {maxCrossings}");
        if (isBroken) return;

        crossings++;
        Debug.Log($"Plank {name} crossed {crossings}/{maxCrossings} times.");

        if (crossings >= maxCrossings)
        {
            Debug.Log($"Crossings ({crossings}) >= maxCrossings ({maxCrossings}). Breaking plank...");
            BreakPlank();
        }
        else
        {
            Debug.Log($"Crossings ({crossings}) < maxCrossings ({maxCrossings}). Not breaking plank yet.");
        }
    }

    private void BreakPlank()
    {
        Debug.Log($"💥 Plank {name} broke in the middle!");
        isBroken = true;

        foreach (var segment in segmentColliders)
        {
            if (segment == null)
            {
                Debug.LogError($"Segment in segmentColliders is null!");
                continue;
            }

            Collider[] colliders = segment.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                Debug.Log($"Disabling collider on {segment.name}: {collider}");
                collider.enabled = false;
            }

            var modifier = segment.GetComponent<NavMeshModifier>();
            if (modifier != null)
            {
                modifier.area = 2; // "Unwalkable" area (adjust the index based on your setup)
                Debug.Log($"Set NavMeshModifier on {segment.name} to Unwalkable area.");
            }
            else
            {
                Debug.LogWarning($"No NavMeshModifier found on {segment.name}!");
            }
        }

        for (int i = 0; i < segmentRigidbodies.Length; i++)
        {
            if (segmentRigidbodies[i] == null)
            {
                Debug.LogError($"Rigidbody at index {i} is null!");
                continue;
            }

            Debug.Log($"Before physics activation on {segmentRigidbodies[i].name}: isKinematic = {segmentRigidbodies[i].isKinematic}, useGravity = {segmentRigidbodies[i].useGravity}");
            segmentRigidbodies[i].isKinematic = false;
            segmentRigidbodies[i].useGravity = true;
            Debug.Log($"After physics activation on {segmentRigidbodies[i].name}: isKinematic = {segmentRigidbodies[i].isKinematic}, useGravity = {segmentRigidbodies[i].useGravity}");

            Vector3 breakForce = (i == 0) ? new Vector3(-2f, -1f, 0f) : new Vector3(2f, -1f, 0f);
            segmentRigidbodies[i].AddForce(breakForce, ForceMode.Impulse);
            segmentRigidbodies[i].AddTorque(new Vector3(0f, 0f, Random.Range(-2f, 2f)), ForceMode.Impulse);
            Debug.Log($"Applied force {breakForce} and torque to {segmentRigidbodies[i].name}");
        }

        StartCoroutine(DestroySegmentsAfterDelay(5f));
    }

    private System.Collections.IEnumerator DestroySegmentsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var segment in segmentColliders)
        {
            Destroy(segment);
        }
    }
}