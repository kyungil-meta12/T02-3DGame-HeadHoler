using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sg_ScoreManager : MonoBehaviour
{
    public static Sg_ScoreManager Inst;

    [System.Serializable]
    public class ScoreLogEntry
    {
        public string label;
        public int amount;
        public int totalAfter;

        public ScoreLogEntry(string label, int amount, int totalAfter)
        {
            this.label = label;
            this.amount = amount;
            this.totalAfter = totalAfter;
        }
    }

    [Header("Score")]
    [SerializeField] private int currentScore = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreTextObj;

    [Header("Popup")]
    [SerializeField] private ScorePopupNotifier scorePopupNotifier;

    private readonly List<ScoreLogEntry> scoreLogList = new List<ScoreLogEntry>();

    public int CurrentScore
    {
        get { return currentScore; }
    }

    public IReadOnlyList<ScoreLogEntry> ScoreLogList
    {
        get { return scoreLogList; }
    }

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        AddScore(amount, "Score Gain");
    }

    public void AddScore(int amount, string label)
    {
        if (amount < 0)
        {
            amount = 0;
        }

        currentScore += amount;
        scoreLogList.Add(new ScoreLogEntry(label, amount, currentScore));

        UpdateScoreUI();

        if (scorePopupNotifier != null)
        {
            scorePopupNotifier.ShowScoreGain(amount);
        }

        Debug.Log($"[Sg_ScoreManager] AddScore: +{amount}, CurrentScore: {currentScore}");
    }

    public void RemoveScore(int amount)
    {
        RemoveScore(amount, "Score Lose");
    }

    public void RemoveScore(int amount, string label)
    {
        if (amount < 0)
        {
            amount = 0;
        }

        currentScore -= amount;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        scoreLogList.Add(new ScoreLogEntry(label, -amount, currentScore));

        UpdateScoreUI();

        if (scorePopupNotifier != null)
        {
            scorePopupNotifier.ShowScoreLose(amount);
        }

        Debug.Log($"[Sg_ScoreManager] RemoveScore: -{amount}, CurrentScore: {currentScore}");
    }

    public void SetScore(int newScore)
    {
        currentScore = newScore;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        UpdateScoreUI();
    }

    public void ClearGameLog()
    {
        currentScore = 0;
        scoreLogList.Clear();
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (scoreTextObj != null)
        {
            scoreTextObj.text = $"Score : {currentScore:N0}";
        }
    }
}