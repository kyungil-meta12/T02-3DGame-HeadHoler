using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_SceneLoader : MonoBehaviour
{
    [Header("Default Target")]
    [SerializeField] private string targetSceneName;

    // Inspector에 적어둔 씬으로 이동
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("[UI_SceneLoader] targetSceneName is empty.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    // 버튼 OnClick에서 문자열 인자로 직접 씬 이름 전달 가능
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[UI_SceneLoader] sceneName is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadCustomizeScene()
    {
        SceneManager.LoadScene("CustomizeScene");
    }

    public void LoadStageScene00()
    {
        SceneManager.LoadScene("StageScene_00");
    }

    public void LoadStageScene01()
    {
        SceneManager.LoadScene("01_StageScene");
    }

    public void LoadStageScene02()
    {
        SceneManager.LoadScene("02_StageScene");
    }

    public void LoadStageScene03()
    {
        SceneManager.LoadScene("03_StageScene");
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}