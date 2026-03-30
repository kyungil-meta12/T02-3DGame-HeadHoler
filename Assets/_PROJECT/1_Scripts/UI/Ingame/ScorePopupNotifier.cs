using Michsky.UI.Heat;
using UnityEngine;

public class ScorePopupNotifier : MonoBehaviour
{
    [Header("Popup Prefab")]
    [SerializeField] private GameObject popupPrefab;

    [Header("Popup Parent")]
    [SerializeField] private Transform popupParent;

    [Header("Text Settings")]
    [SerializeField] private string gainTextFormat = "+{0} Score";
    [SerializeField] private string loseTextFormat = "-{0} Score";

    [Header("Popup Settings")]
    [SerializeField] private float popupMinimizeAfter = 1.5f;

    public void ShowScoreGain(int amount)
    {
        SpawnPopup(string.Format(gainTextFormat, amount));
    }

    public void ShowScoreLose(int amount)
    {
        SpawnPopup(string.Format(loseTextFormat, amount));
    }

    public void ShowCustomMessage(string message)
    {
        SpawnPopup(message);
    }

    private void SpawnPopup(string message)
    {
        if (popupPrefab == null)
        {
            Debug.LogWarning("[ScorePopupNotifier] popupPrefab is null.");
            return;
        }

        Transform parentToUse = popupParent != null ? popupParent : transform;

        GameObject popupObj = Instantiate(popupPrefab, parentToUse);
        QuestItem questItem = popupObj.GetComponent<QuestItem>();

        if (questItem == null)
        {
            questItem = popupObj.GetComponentInChildren<QuestItem>(true);
        }

        if (questItem == null)
        {
            Debug.LogWarning("[ScorePopupNotifier] QuestItem component not found in popupPrefab.");
            Destroy(popupObj);
            return;
        }

        questItem.useLocalization = false;
        questItem.questText = message;
        questItem.minimizeAfter = popupMinimizeAfter;
        questItem.afterMinimize = QuestItem.AfterMinimize.Destroy;

        questItem.UpdateUI();
        questItem.AnimateQuest();
    }
}