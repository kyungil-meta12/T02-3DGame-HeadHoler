using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.UI.Heat;

public class UI_MainMenuStartButton : MonoBehaviour
{
    [Header("Heat UI Button")]
    [SerializeField] private BoxButtonManager boxButton;

    [Header("Stage Scene Names")]
    [SerializeField] private string stage01SceneName = "01_StageScene";
    [SerializeField] private string stage02SceneName = "02_StageScene";
    [SerializeField] private string stage03SceneName = "03_StageScene";
    [SerializeField] private string stage11SceneName = "11_StageScene";
    [SerializeField] private string stage12SceneName = "12_StageScene";
    [SerializeField] private string stage13SceneName = "13_StageScene";

    [Header("Clear Achievement Titles")]
    [SerializeField] private string stage01ClearAchievement = "Easy 1 클리어";
    [SerializeField] private string stage02ClearAchievement = "Easy 2 클리어";
    [SerializeField] private string stage03ClearAchievement = "Easy 3 클리어";
    [SerializeField] private string stage11ClearAchievement = "Normal 1 클리어";
    [SerializeField] private string stage12ClearAchievement = "Normal 2 클리어";
    [SerializeField] private string stage13ClearAchievement = "Normal 3 클리어";

    [Header("Button Text")]
    [SerializeField] private string defaultTitle = "게임 시작";
    [SerializeField] private string defaultDescription = "현재 진행 가능한 다음 스테이지로 이동합니다.";
    [SerializeField] private string allClearTitle = "모든 챕터 클리어";
    [SerializeField] private string allClearDescription = "마지막 스테이지를 다시 플레이합니다.";
    [SerializeField] private bool replayLastStageWhenAllCleared = true;

    private void Reset()
    {
        boxButton = GetComponent<BoxButtonManager>();
    }

    private void Awake()
    {
        if (boxButton == null)
            boxButton = GetComponent<BoxButtonManager>();
    }

    private void OnEnable()
    {
        RefreshButtonUI();
    }

    public void OnClickStartGame()
    {
        string nextStageScene = GetNextStageSceneName();

        if (string.IsNullOrEmpty(nextStageScene))
        {
            Debug.LogWarning("[UI_MainMenuStartButton] 시작할 스테이지가 없습니다.");
            return;
        }

        if (Sg_AchievementRuntime.Inst != null)
        {
            Sg_AchievementRuntime.Inst.SetSelectedStage(nextStageScene);
        }
        else
        {
            Debug.LogWarning("[UI_MainMenuStartButton] Sg_AchievementRuntime.Inst 가 없습니다.");
        }

        SceneManager.LoadScene(nextStageScene);
    }

    public void RefreshButtonUI()
    {
        if (boxButton == null)
        {
            Debug.LogWarning("[UI_MainMenuStartButton] BoxButtonManager 참조가 없습니다.");
            return;
        }

        string title = defaultTitle;
        string description = defaultDescription;

        if (!IsAchievementUnlocked(stage01ClearAchievement))
        {
            title = "게임 시작 - Easy 1스테이지";
            description = "다음 진행 스테이지인 01스테이지를 시작합니다.";
        }
        else if (!IsAchievementUnlocked(stage02ClearAchievement))
        {
            title = "게임 시작 - Easy 2스테이지";
            description = "다음 진행 스테이지인 02스테이지를 시작합니다.";
        }
        else if (!IsAchievementUnlocked(stage03ClearAchievement))
        {
            title = "게임 시작 - Easy 3스테이지";
            description = "다음 진행 스테이지인 03스테이지를 시작합니다.";
        }
        else if (!IsAchievementUnlocked(stage11ClearAchievement))
        {
            title = "게임 시작 - Normal 1스테이지";
            description = "다음 진행 스테이지인 11스테이지를 시작합니다.";
        }
        else if (!IsAchievementUnlocked(stage12ClearAchievement))
        {
            title = "게임 시작 - Normal 2스테이지";
            description = "다음 진행 스테이지인 12스테이지를 시작합니다.";
        }
        else if (!IsAchievementUnlocked(stage13ClearAchievement))
        {
            title = "게임 시작 - Normal 3스테이지";
            description = "다음 진행 스테이지인 13스테이지를 시작합니다.";
        }
        else
        {
            title = replayLastStageWhenAllCleared ? allClearTitle : defaultTitle;
            description = replayLastStageWhenAllCleared ? allClearDescription : defaultDescription;
        }

        boxButton.buttonTitle = title;
        boxButton.buttonDescription = description;
        boxButton.UpdateUI();
    }

    private string GetNextStageSceneName()
    {
        if (!IsAchievementUnlocked(stage01ClearAchievement))
            return stage01SceneName;

        if (!IsAchievementUnlocked(stage02ClearAchievement))
            return stage02SceneName;

        if (!IsAchievementUnlocked(stage03ClearAchievement))
            return stage03SceneName;

        if (!IsAchievementUnlocked(stage11ClearAchievement))
            return stage11SceneName;

        if (!IsAchievementUnlocked(stage12ClearAchievement))
            return stage12SceneName;

        if (!IsAchievementUnlocked(stage13ClearAchievement))
            return stage13SceneName;

        if (replayLastStageWhenAllCleared)
            return stage13SceneName;

        return string.Empty;
    }

    private bool IsAchievementUnlocked(string achievementTitle)
    {
        return PlayerPrefs.GetString("ACH_" + achievementTitle, "false") == "true";
    }

    [ContextMenu("Refresh Start Button UI")]
    public void RefreshStartButtonUIInEditor()
    {
        RefreshButtonUI();
    }

    [ContextMenu("Reset Clear Progress For Test")]
    public void ResetClearProgressForTest()
    {
        PlayerPrefs.DeleteKey("ACH_" + stage01ClearAchievement);
        PlayerPrefs.DeleteKey("ACH_" + stage02ClearAchievement);
        PlayerPrefs.DeleteKey("ACH_" + stage03ClearAchievement);
        PlayerPrefs.DeleteKey("ACH_" + stage11ClearAchievement);
        PlayerPrefs.DeleteKey("ACH_" + stage12ClearAchievement);
        PlayerPrefs.DeleteKey("ACH_" + stage13ClearAchievement);
        PlayerPrefs.Save();

        RefreshButtonUI();
        Debug.Log("[UI_MainMenuStartButton] 클리어 진행도를 초기화했습니다.");
    }
}