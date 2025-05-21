using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;
    [SerializeField] private Toggle postProcessingToggle;
    [SerializeField] private Toggle shadowToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vSyncToggle;

    void Start()
    {
        Debug.Log("OptionsMenu Start called");
        if (GlobalSettingsManager.Instance == null)
        {
            Debug.LogError("GlobalSettingsManager.Instance is null in OptionsMenu Start");
            return;
        }

        // Initialize antialiasing dropdown
        if (antiAliasingDropdown != null)
        {
            antiAliasingDropdown.onValueChanged.AddListener(SetAA);
            antiAliasingDropdown.value = GlobalSettingsManager.Instance.GetAntiAliasing();
            Debug.Log($"AntiAliasingDropdown initialized with value: {antiAliasingDropdown.value}");
        }
        else
        {
            Debug.LogWarning("AntiAliasingDropdown is not assigned in OptionsMenu", this);
        }

        // Initialize post-processing toggle
        if (postProcessingToggle != null)
        {
            postProcessingToggle.onValueChanged.AddListener(SetPostFX);
            postProcessingToggle.isOn = GlobalSettingsManager.Instance.GetPostProcessing();
            Debug.Log($"PostProcessingToggle initialized with isOn: {postProcessingToggle.isOn}");
        }
        else
        {
            Debug.LogWarning("PostProcessingToggle is not assigned in OptionsMenu", this);
        }

        // Initialize shadows toggle
        if (shadowToggle != null)
        {
            shadowToggle.onValueChanged.AddListener(SetShadows);
            shadowToggle.isOn = GlobalSettingsManager.Instance.GetShadows();
            Debug.Log($"ShadowToggle initialized with isOn: {shadowToggle.isOn}");
        }
        else
        {
            Debug.LogWarning("ShadowToggle is not assigned in OptionsMenu", this);
        }

        // Initialize resolution dropdown
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            SetupResolutionDropdown();
            Debug.Log($"ResolutionDropdown initialized with value: {resolutionDropdown.value}");
        }
        else
        {
            Debug.LogWarning("ResolutionDropdown is not assigned in OptionsMenu", this);
        }

        // Initialize fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            fullscreenToggle.isOn = GlobalSettingsManager.Instance.GetFullscreen();
            Debug.Log($"FullscreenToggle initialized with isOn: {fullscreenToggle.isOn}");
        }
        else
        {
            Debug.LogWarning("FullscreenToggle is not assigned in OptionsMenu", this);
        }

        // Initialize VSync toggle
        if (vSyncToggle != null)
        {
            vSyncToggle.onValueChanged.AddListener(SetVSync);
            vSyncToggle.isOn = GlobalSettingsManager.Instance.GetVSync();
            Debug.Log($"VSyncToggle initialized with isOn: {vSyncToggle.isOn}");
        }
        else
        {
            Debug.LogWarning("VSyncToggle is not assigned in OptionsMenu", this);
        }
    }

    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        Resolution[] resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width}x{resolutions[i].height}";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void SetAA(int index)
    {
        Debug.Log($"SetAA called with index: {index}");
        GlobalSettingsManager.Instance.SetAntiAliasing(index);
    }

    void SetPostFX(bool enabled)
    {
        Debug.Log($"SetPostFX called with enabled: {enabled}");
        GlobalSettingsManager.Instance.TogglePostProcessing(enabled);
    }

    void SetShadows(bool enabled)
    {
        Debug.Log($"SetShadows called with enabled: {enabled}");
        GlobalSettingsManager.Instance.ToggleShadows(enabled);
    }

    void SetResolution(int index)
    {
        Debug.Log($"SetResolution called with index: {index}");
        GlobalSettingsManager.Instance.SetResolution(index);
    }

    void SetFullscreen(bool enabled)
    {
        Debug.Log($"SetFullscreen called with enabled: {enabled}");
        GlobalSettingsManager.Instance.SetFullscreen(enabled);
    }

    void SetVSync(bool enabled)
    {
        Debug.Log($"SetVSync called with enabled: {enabled}");
        GlobalSettingsManager.Instance.SetVSync(enabled);
    }
}