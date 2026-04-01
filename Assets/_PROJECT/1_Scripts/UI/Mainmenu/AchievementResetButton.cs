using UnityEngine;
using Michsky.UI.Heat;

public class AchievementResetButton : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;

    public void ResetAllAchievements()
    {
        string[] titles =
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
            "테스트 스테이지 1000점 달성"
        };

        for (int i = 0; i < titles.Length; i++)
        {
            PlayerPrefs.DeleteKey("ACH_" + titles[i]);
        }

        PlayerPrefs.Save();

        if (achievementManager != null)
            achievementManager.InitializeItems();

        Debug.Log("[AchievementResetButton] All achievements reset.");
    }
}