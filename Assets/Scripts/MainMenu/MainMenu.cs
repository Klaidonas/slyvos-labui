using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel; // OptionsPanel on OptionsMenu Canvas
    [SerializeField] private GameObject mainMenuPanel; // MainMenuPanel on Main Canvas
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        Debug.Log("MainMenu Start called");
        if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
        else Debug.LogWarning("NewGameButton is not assigned in MainMenu", this);

        if (loadGameButton != null) loadGameButton.onClick.AddListener(OnLoadGame);
        else Debug.LogWarning("LoadGameButton is not assigned in MainMenu", this);

        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptions);
        else Debug.LogWarning("OptionsButton is not assigned in MainMenu", this);

        if (exitButton != null) exitButton.onClick.AddListener(OnExit);
        else Debug.LogWarning("ExitButton is not assigned in MainMenu", this);
    }

    public void OnNewGame()
    {
        Debug.Log("NewGame button clicked");
        SceneManager.LoadScene("GameScene"); // Replace with your scene name
    }

    public void OnLoadGame()
    {
        Debug.Log("LoadGame button clicked - implement save/load system");
    }

    public void OnOptions()
    {
        Debug.Log("MainMenu OnOptions called");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            Debug.Log("OptionsPanel activated");
        }
        else
        {
            Debug.LogWarning("OptionsPanel is not assigned in MainMenu", this);
        }
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("MainMenuPanel deactivated");
        }
        else
        {
            Debug.LogWarning("MainMenuPanel is not assigned in MainMenu", this);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("ShowMainMenu called - MainMenuPanel activated");
        }
        else
        {
            Debug.LogWarning("MainMenuPanel is not assigned in ShowMainMenu", this);
        }
    }
}