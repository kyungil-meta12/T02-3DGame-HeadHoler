using UnityEngine;
using Michsky.UI.Heat;

public class Sg_AchievementRuntime : MonoBehaviour
{
    public static Sg_AchievementRuntime Inst;

    [Header("Selected Stage")]
    [SerializeField] private string selectedStageId;

    [Header("Stage Score Thresholds")]
    [SerializeField] private int testScoreThreshold = 4000;
    [SerializeField] private int stage00ScoreThreshold = 2000;
    [SerializeField] private int stage01ScoreThreshold = 3000;
    [SerializeField] private int stage02ScoreThreshold = 4000;
    [SerializeField] private int stage03ScoreThreshold = 5000;

    public string SelectedStageId => selectedStageId;

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 메인메뉴에서 스테이지 선택 시 호출
    /// </summary>
    public void SetSelectedStage(string stageId)
    {
        selectedStageId = stageId;
        Debug.Log($"[Sg_AchievementRuntime] Selected Stage: {selectedStageId}");
    }

    /// <summary>
    /// 게임 종료 시 호출
    /// cleared = true  -> 스테이지 클리어
    /// cleared = false -> 실패
    /// </summary>
    public void ProcessStageResult(int score, bool cleared)
    {
        Debug.Log($"[Sg_AchievementRuntime] ProcessStageResult - Stage: {selectedStageId}, Score: {score}, Cleared: {cleared}");

        switch (selectedStageId)
        {
            case "Test":
                if (cleared)
                {
                    AchievementManager.SetAchievement("테스트 스테이지 클리어", true);
                    UpdateChapterProgress("Test", "StageScene_00");
                }

                if (cleared && score >= testScoreThreshold)
                    AchievementManager.SetAchievement("테스트 스테이지 4000점 달성", true);
                break;

            case "StageScene_00":
                if (cleared)
                {
                    AchievementManager.SetAchievement("튜토리얼 스테이지 클리어", true);
                    UpdateChapterProgress("StageScene_00", "01_StageScene");
                }

                if (cleared && score >= stage00ScoreThreshold)
                    AchievementManager.SetAchievement("튜토리얼 2000점 달성", true);
                break;

            case "01_StageScene":
                if (cleared)
                {
                    AchievementManager.SetAchievement("1스테이지 클리어", true);
                    UpdateChapterProgress("01_StageScene", "02_StageScene");
                }

                if (cleared && score >= stage01ScoreThreshold)
                    AchievementManager.SetAchievement("1스테이지 3000점 달성", true);
                break;

            case "02_StageScene":
                if (cleared)
                {
                    AchievementManager.SetAchievement("2스테이지 클리어", true);
                    UpdateChapterProgress("02_StageScene", "03_StageScene");
                }

                if (cleared && score >= stage02ScoreThreshold)
                    AchievementManager.SetAchievement("2스테이지 4000점 달성", true);
                break;

            case "03_StageScene":
                if (cleared)
                {
                    AchievementManager.SetAchievement("3스테이지 클리어", true);
                    UpdateChapterProgress("03_StageScene", null);
                }

                if (cleared && score >= stage03ScoreThreshold)
                    AchievementManager.SetAchievement("3스테이지 5000점 달성", true);
                break;

            default:
                Debug.LogWarning($"[Sg_AchievementRuntime] Unknown stage id: {selectedStageId}");
                break;
        }

        PlayerPrefs.Save();
    }

    private void UpdateChapterProgress(string completedChapterId, string nextCurrentChapterId)
    {
        if (!string.IsNullOrEmpty(completedChapterId))
        {
            ChapterManager.SetCompleted(completedChapterId);
        }

        if (!string.IsNullOrEmpty(nextCurrentChapterId))
        {
            ChapterManager.SetCurrent(nextCurrentChapterId);
        }
    }
}