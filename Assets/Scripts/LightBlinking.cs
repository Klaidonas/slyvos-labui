using UnityEngine;
using UnityEngine.Rendering; // Required for LensFlareComponentSRP

public class BlinkingLamp : MonoBehaviour
{
    [Header("References")]
    public Light lampLight;
    public LensFlareComponentSRP lensFlare; // New SRP flare component

    [Header("Settings")]
    public float blinkInterval = 0.5f;
    public bool startOn = true;
    [Range(0, 1)] public float flareIntensity = 1f;

    private float timer;
    private bool isOn;

    void Start()
    {
        // Auto-get components if not assigned
        if (lampLight == null) lampLight = GetComponent<Light>();
        if (lensFlare == null) lensFlare = GetComponent<LensFlareComponentSRP>();

        isOn = startOn;
        UpdateLightState();
        timer = blinkInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            isOn = !isOn;
            UpdateLightState();
            timer = blinkInterval;
        }
    }

    void UpdateLightState()
    {
        // Update light
        if (lampLight != null) lampLight.enabled = isOn;

        // Update flare
        if (lensFlare != null) lensFlare.intensity = isOn ? flareIntensity : 0f;
    }

    // Public controls
    public void Toggle() => isOn = !isOn;
    public void SetActive(bool state) => isOn = state;
}