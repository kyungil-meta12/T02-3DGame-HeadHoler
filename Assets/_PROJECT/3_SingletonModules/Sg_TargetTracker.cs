using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.Heat;

public class Sg_TargetTracker : MonoBehaviour
{
    public static Sg_TargetTracker Inst;

    [Header("Targets")]
    [SerializeField] private List<RagdollController> allTargets = new List<RagdollController>();

    [Header("Quest UI")]
    [SerializeField] private QuestItem targetQuestItem;
    [SerializeField] private QuestItemSafeTrigger targetQuestTrigger;
    [SerializeField] private string targetTextFormat = "남은 타겟 : {0}";

    [Header("Result Popup")]
    [SerializeField] private ResultPopupController resultPopupController;

    [Header("HUD Target Slots")]
    [SerializeField] private HudTargetSlotUI[] hudTargetSlots;

    private bool isGameCleared = false;

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        Debug.Log("[Sg_TargetTracker] Created instance.");
    }

    private void OnDestroy()
    {
        if (Inst == this)
            Inst = null;
    }

    private void Start()
    {
        CleanupNullTargets();
        RefreshTargetUI();
        RefreshHudTargetSlots();
    }

    public void RegisterTarget(RagdollController target)
    {
        if (target == null)
            return;

        if (target.IsClearTarget == false)
            return;

        if (allTargets.Contains(target))
            return;

        allTargets.Add(target);
        RefreshTargetUI();

        Debug.Log($"[Sg_TargetTracker] RegisterTarget: {target.name} / Alive = {GetAliveTargetCount()}");
    }

    public void NotifyTargetKilled(RagdollController target)
    {
        if (target == null)
            return;

        if (isGameCleared)
            return;

        RefreshTargetUI();
        RefreshHudTargetSlots();

        Debug.Log($"[Sg_TargetTracker] NotifyTargetKilled: {target.name} / Alive = {GetAliveTargetCount()}");

        CheckClear();
    }

    public int GetAliveTargetCount()
    {
        CleanupNullTargets();

        int count = 0;

        for (int i = 0; i < allTargets.Count; i++)
        {
            RagdollController target = allTargets[i];

            if (target == null)
                continue;

            if (target.IsClearTarget && target.IsDead == false)
                count++;
        }

        return count;
    }

    public int GetTotalTargetCount()
    {
        CleanupNullTargets();
        return allTargets.Count;
    }

    private void RefreshTargetUI()
    {
        if (targetQuestItem == null)
            return;

        int aliveCount = GetAliveTargetCount();
        string text = string.Format(targetTextFormat, aliveCount);

        targetQuestItem.questText = text;

        if (targetQuestTrigger != null)
        {
            targetQuestTrigger.SetQuestTextAndShow(text);
        }
        else
        {
            targetQuestItem.UpdateUI();
        }
    }

    private void CheckClear()
    {
        if (isGameCleared)
            return;

        if (GetAliveTargetCount() <= 0)
        {
            isGameCleared = true;
            GameClear();
        }
    }

    private void GameClear()
    {
        Debug.Log("Game Clear.");

        if (resultPopupController != null)
        {
            resultPopupController.ShowSuccess();
        }
        else
        {
            Debug.LogWarning("[Sg_TargetTracker] ResultPopupController is null.");
        }
    }

    private void CleanupNullTargets()
    {
        for (int i = allTargets.Count - 1; i >= 0; i--)
        {
            if (allTargets[i] == null)
                allTargets.RemoveAt(i);
        }
    }

    private void RefreshHudTargetSlots()
    {
        if (hudTargetSlots == null)
            return;

        for (int i = 0; i < hudTargetSlots.Length; i++)
        {
            if (hudTargetSlots[i] != null)
                hudTargetSlots[i].Refresh();
        }
    }
}

