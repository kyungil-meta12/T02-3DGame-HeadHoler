using UnityEngine;
using Michsky.UI.Heat;

public class AchievementResetButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private UI_MainMenuStartButton mainMenuStartButton;
    [SerializeField] private ChapterManager chapterManager;

    /// <summary>
    /// 업적 + 챕터 진행도만 초기화
    /// </summary>
    public void ResetAllAchievementsAndProgress()
    {
        ResetAchievementPrefs();
        ResetChapterStatePrefs();
        ResetSelectedStage();

        PlayerPrefs.Save();
        RefreshAllUI();

        Debug.Log("[AchievementResetButton] Achievements and chapter progress reset.");
    }

    /// <summary>
    /// PlayerPrefs 전체 삭제
    /// 옵션, 업적, 진행도 등 전부 삭제됨
    /// </summary>
    public void ResetAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ResetSelectedStage();
        RefreshAllUI();

        Debug.Log("[AchievementResetButton] All PlayerPrefs deleted.");
    }

    private void ResetAchievementPrefs()
    {
        string[] achievementTitles =
        {
            "튜토리얼 스테이지 클리어",
            "튜토리얼 2000점 달성",
            "1스테이지 클리어",
            "1스테이지 3000점 달성",
            "2스테이지 클리어",
            "2스테이지 4000점 달성",
            "3스테이지 클리어",
            "3스테이지 5000점 달성",
            "테스트 스테이지 클리어",
            "테스트 스테이지 4000점 달성"
        };

        for (int i = 0; i < achievementTitles.Length; i++)
        {
            PlayerPrefs.DeleteKey("ACH_" + achievementTitles[i]);
        }
    }

    private void ResetChapterStatePrefs()
    {
        string[] chapterIds =
        {
            "Test",
            "StageScene_00",
            "01_StageScene",
            "02_StageScene",
            "03_StageScene"
        };

        for (int i = 0; i < chapterIds.Length; i++)
        {
            PlayerPrefs.DeleteKey("ChapterState_" + chapterIds[i]);
        }
    }

    private void ResetSelectedStage()
    {
        if (Sg_AchievementRuntime.Inst != null)
        {
            Sg_AchievementRuntime.Inst.SetSelectedStage(string.Empty);
        }
    }

    private void RefreshAllUI()
    {
        if (achievementManager != null)
            achievementManager.InitializeItems();

        if (chapterManager != null)
            chapterManager.InitializeChapters();

        if (mainMenuStartButton != null)
            mainMenuStartButton.RefreshButtonUI();
    }
}