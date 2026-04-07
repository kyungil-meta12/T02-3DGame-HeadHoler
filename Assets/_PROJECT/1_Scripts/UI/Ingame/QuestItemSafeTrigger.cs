using System.Collections;
using UnityEngine;
using Michsky.UI.Heat;

public class QuestItemSafeTrigger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private QuestItem questItem;

    [Header("Options")]
    [SerializeField] private bool forceUpdateTextBeforeShow = true;

    private Coroutine showRoutine;

    public void ShowQuest()
    {
        if (questItem == null)
        {
            Debug.LogWarning("[QuestItemSafeTrigger] questItem is null.");
            return;
        }

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(CoShowQuestSafe());
    }

    private IEnumerator CoShowQuestSafe()
    {
        GameObject targetObj = questItem.gameObject;

        if (targetObj.activeSelf == false)
            targetObj.SetActive(true);

        // 비활성 -> 활성 직후 바로 호출하지 말고 한 프레임 기다림
        yield return null;

        if (questItem != null)
        {
            if (forceUpdateTextBeforeShow)
                questItem.UpdateUI();

            questItem.AnimateQuest();
        }

        showRoutine = null;
    }

    public void HideQuest()
    {
        if (questItem == null)
        {
            Debug.LogWarning("[QuestItemSafeTrigger] questItem is null.");
            return;
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        if (questItem.gameObject.activeSelf)
            questItem.MinimizeQuest();
    }

    public void SetQuestTextAndShow(string text)
    {
        if (questItem == null)
        {
            Debug.LogWarning("[QuestItemSafeTrigger] questItem is null.");
            return;
        }

        questItem.questText = text;
        ShowQuest();
    }
}