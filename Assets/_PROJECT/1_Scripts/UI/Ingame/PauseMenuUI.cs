using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Option")]
    [SerializeField] private bool pauseTimeScale = true;

    private bool isOpen;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[PauseMenuUI] settingsPanel is not assigned.");
            return;
        }

        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);

        if (pauseTimeScale)
        {
            Time.timeScale = isOpen ? 0f : 1f;
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        isOpen = true;
        settingsPanel.SetActive(true);

        if (pauseTimeScale)
            Time.timeScale = 0f;
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        isOpen = false;
        settingsPanel.SetActive(false);

        if (pauseTimeScale)
            Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        CloseSettings();
    }

    public void GoToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void OnDisable()
    {
        if (pauseTimeScale)
            Time.timeScale = 1f;
    }
}