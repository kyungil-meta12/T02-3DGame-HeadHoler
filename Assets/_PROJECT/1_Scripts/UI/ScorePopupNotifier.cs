using Michsky.UI.Heat;
using UnityEngine;

public class ScorePopupNotifier : MonoBehaviour
{
    [Header("Quest Item")]
    [SerializeField] private QuestItem questItem;

    [Header("Text Settings")]
    [SerializeField] private string gainTextFormat = "+{0} Score";
    [SerializeField] private string loseTextFormat = "-{0} Score";

    private void Awake()
    {
        if (questItem == null)
        {
            questItem = GetComponentInChildren<QuestItem>(true);
        }
    }

    public void ShowScoreGain(int amount)
    {
        if (questItem == null)
        {
            return;
        }

        questItem.questText = string.Format(gainTextFormat, amount);
        questItem.UpdateUI();
        questItem.AnimateQuest();
    }

    public void ShowScoreLose(int amount)
    {
        if (questItem == null)
        {
            return;
        }

        questItem.questText = string.Format(loseTextFormat, amount);
        questItem.UpdateUI();
        questItem.AnimateQuest();
    }

    public void ShowCustomMessage(string message)
    {
        if (questItem == null)
        {
            return;
        }

        questItem.questText = message;
        questItem.UpdateUI();
        questItem.AnimateQuest();
    }
}