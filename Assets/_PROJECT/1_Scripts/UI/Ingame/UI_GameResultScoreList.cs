using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Michsky.UI.Heat;

public class UI_GameResultScoreList : MonoBehaviour
{
    [Header("Result Text")]
    [SerializeField] private TMP_Text resultText;

    [Header("Final Score Description")]
    [SerializeField] private SettingsDescriptionManager finalScoreDescription;

    [Header("Description Text")]
    [SerializeField] private string finalScoreTitle = "Final Score";
    [SerializeField] private string finalScoreFormat = "Final Score : {0:N0}";
    [SerializeField] private string emptyText = "No score data.";

    private void OnEnable()
    {
        Refresh();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (resultText == null)
        {
            Debug.LogWarning("[UI_GameResultScoreList] resultText is null.");
            return;
        }

        resultText.enabled = true;
        resultText.gameObject.SetActive(true);

        if (Sg_ScoreManager.Inst == null)
        {
            Debug.LogWarning("[UI_GameResultScoreList] Sg_ScoreManager.Inst is null.");
            resultText.text = emptyText;
            return;
        }

        IReadOnlyList<Sg_ScoreManager.ScoreLogEntry> logs = Sg_ScoreManager.Inst.ScoreLogList;
        resultText.text = BuildScoreText(logs);

        UpdateFinalScoreDescription(Sg_ScoreManager.Inst.CurrentScore);
    }

    private string BuildScoreText(IReadOnlyList<Sg_ScoreManager.ScoreLogEntry> logs)
    {
        if (logs == null || logs.Count == 0)
            return emptyText;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < logs.Count; i++)
        {
            var entry = logs[i];
            string amountText = entry.amount >= 0 ? $"+{entry.amount:N0}" : $"{entry.amount:N0}";
            sb.AppendLine($"{entry.label}   {amountText}");
        }

        return sb.ToString();
    }

    private void UpdateFinalScoreDescription(int finalScore)
    {
        if (finalScoreDescription == null)
        {
            Debug.LogWarning("[UI_GameResultScoreList] finalScoreDescription is null.");
            return;
        }

        string finalDescription = string.Format(finalScoreFormat, finalScore);
        finalScoreDescription.UpdateUI(finalScoreTitle, finalDescription, null);
    }
}