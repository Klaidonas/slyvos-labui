using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private MainMenu mainMenu;

    void Start()
    {
        Debug.Log("OptionsUI Start called");
        // Show Video tab by default
        ShowVideo();
    }

    public void ShowVideo()
    {
        if (videoPanel != null) videoPanel.SetActive(true);
        else Debug.LogWarning("VideoPanel is not assigned in OptionsUI", this);

        if (audioPanel != null) audioPanel.SetActive(false);
        else Debug.LogWarning("AudioPanel is not assigned in OptionsUI", this);

        if (controlsPanel != null) controlsPanel.SetActive(false);
        else Debug.LogWarning("ControlsPanel is not assigned in OptionsUI", this);

        Debug.Log("Video tab activated");
    }

    public void ShowAudio()
    {
        if (videoPanel != null) videoPanel.SetActive(false);
        else Debug.LogWarning("VideoPanel is not assigned in OptionsUI", this);

        if (audioPanel != null) audioPanel.SetActive(true);
        else Debug.LogWarning("AudioPanel is not assigned in OptionsUI", this);

        if (controlsPanel != null) controlsPanel.SetActive(false);
        else Debug.LogWarning("ControlsPanel is not assigned in OptionsUI", this);

        Debug.Log("Audio tab activated");
    }

    public void ShowControls()
    {
        if (videoPanel != null) videoPanel.SetActive(false);
        else Debug.LogWarning("VideoPanel is not assigned in OptionsUI", this);

        if (audioPanel != null) audioPanel.SetActive(false);
        else Debug.LogWarning("AudioPanel is not assigned in OptionsUI", this);

        if (controlsPanel != null) controlsPanel.SetActive(true);
        else Debug.LogWarning("ControlsPanel is not assigned in OptionsUI", this);

        Debug.Log("Controls tab activated");
    }

    public void OnBack()
    {
        Debug.Log("Back button clicked");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            Debug.Log("OptionsPanel deactivated");
        }
        else
        {
            Debug.LogWarning("OptionsPanel is not assigned in OptionsUI", this);
        }
        if (mainMenu != null)
        {
            mainMenu.ShowMainMenu();
            Debug.Log("OnBack called, showing main menu");
        }
        else
        {
            Debug.LogWarning("MainMenu is not assigned in OptionsUI", this);
        }
    }
}