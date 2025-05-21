using UnityEngine;

public class GlobalPostProcessingPersist : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}