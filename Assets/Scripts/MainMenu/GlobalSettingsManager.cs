using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlobalSettingsManager : MonoBehaviour
{
    public static GlobalSettingsManager Instance { get; private set; }
    private Resolution[] resolutions;
    private Camera mainCamera;
    private PostProcessVolume globalPostProcessVolume;

    void Awake()
    {
        Debug.Log("GlobalSettingsManager Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            resolutions = Screen.resolutions;
            Debug.Log($"GlobalSettingsManager singleton initialized, resolutions count: {resolutions.Length}");

            // Find the global PostProcessVolume
            GameObject globalPostObject = GameObject.Find("GlobalPostProcessing");
            if (globalPostObject != null)
            {
                globalPostProcessVolume = globalPostObject.GetComponent<PostProcessVolume>();
                if (globalPostProcessVolume == null)
                {
                    Debug.LogWarning("GlobalPostProcessing GameObject found but missing PostProcessVolume component");
                }
                else
                {
                    Debug.Log("GlobalPostProcessing found and PostProcessVolume assigned");
                }
            }
            else
            {
                Debug.LogWarning("GlobalPostProcessing GameObject not found in the scene");
            }

            LoadSettings();
        }
        else
        {
            Debug.LogWarning("Duplicate GlobalSettingsManager found, destroying this instance");
            Destroy(gameObject);
        }
    }

    public void RegisterCamera(Camera camera)
    {
        if (camera != null)
        {
            mainCamera = camera;
            Debug.Log($"Camera registered successfully: {camera.name}");
            ApplySettings();
        }
        else
        {
            Debug.LogError("Attempted to register a null camera");
        }
    }

    void LoadSettings()
    {
        SetAntiAliasing(PlayerPrefs.GetInt("AntiAliasing", 2));
        TogglePostProcessing(PlayerPrefs.GetInt("PostProcessing", 1) == 1);
        ToggleShadows(PlayerPrefs.GetInt("Shadows", 1) == 1);
        SetResolution(PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1));
        SetFullscreen(PlayerPrefs.GetInt("Fullscreen", 1) == 1);
        SetVSync(PlayerPrefs.GetInt("VSync", 0) == 1);
    }

    void ApplySettings()
    {
        SetAntiAliasing(GetAntiAliasing());
        TogglePostProcessing(GetPostProcessing());
        ToggleShadows(GetShadows());
        SetResolution(GetResolutionIndex());
        SetFullscreen(GetFullscreen());
        SetVSync(GetVSync());
    }

    public void SetAntiAliasing(int index)
    {
        QualitySettings.antiAliasing = index switch
        {
            1 => 2, // MSAA 2x
            2 => 4, // MSAA 4x
            3 => 8, // MSAA 8x
            _ => 0, // None
        };
        if (mainCamera != null)
        {
            mainCamera.allowMSAA = index > 0;
            Debug.Log($"Camera MSAA enabled: {mainCamera.allowMSAA}");
        }
        Debug.Log($"Set AntiAliasing: {index}, Level: {QualitySettings.antiAliasing}");
        PlayerPrefs.SetInt("AntiAliasing", index);
    }

    public int GetAntiAliasing()
    {
        return PlayerPrefs.GetInt("AntiAliasing", 2);
    }

    public void TogglePostProcessing(bool enabled)
    {
        if (globalPostProcessVolume != null)
        {
            globalPostProcessVolume.enabled = enabled;
            Debug.Log($"PostProcessVolume enabled: {enabled}");
        }
        else
        {
            Debug.LogError("Global PostProcessVolume is null");
        }
        PlayerPrefs.SetInt("PostProcessing", enabled ? 1 : 0);
    }

    public bool GetPostProcessing()
    {
        return PlayerPrefs.GetInt("PostProcessing", 1) == 1;
    }

    public void ToggleShadows(bool enabled)
    {
        QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
        Debug.Log($"Set Shadows: {enabled}, ShadowQuality: {QualitySettings.shadows}");
        PlayerPrefs.SetInt("Shadows", enabled ? 1 : 0);
    }

    public bool GetShadows()
    {
        return PlayerPrefs.GetInt("Shadows", 1) == 1;
    }

    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            Debug.Log($"Set Resolution: {res.width}x{res.height}");
            PlayerPrefs.SetInt("ResolutionIndex", index);
        }
    }

    public int GetResolutionIndex()
    {
        return PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
    }

    public void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
        Debug.Log($"Set Fullscreen: {enabled}");
        PlayerPrefs.SetInt("Fullscreen", enabled ? 1 : 0);
    }

    public bool GetFullscreen()
    {
        return PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        Debug.Log($"Set VSync: {enabled}");
        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
    }

    public bool GetVSync()
    {
        return PlayerPrefs.GetInt("VSync", 0) == 1;
    }
}