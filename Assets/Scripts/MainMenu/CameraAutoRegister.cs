using UnityEngine;

public class CameraAutoRegister : MonoBehaviour
{
    void Start()
    {
        Debug.Log("CameraAutoRegister Start called");
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            if (GlobalSettingsManager.Instance != null)
            {
                GlobalSettingsManager.Instance.RegisterCamera(camera);
                Debug.Log($"Camera registered: {camera.name}");
            }
            else
            {
                Debug.LogError("GlobalSettingsManager.Instance is null");
            }
        }
        else
        {
            Debug.LogError("No Camera component found on this GameObject");
        }
    }
}