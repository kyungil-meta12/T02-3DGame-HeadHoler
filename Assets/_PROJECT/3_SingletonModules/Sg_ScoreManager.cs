using TMPro;
using UnityEngine;

public class Sg_ScoreManager : MonoBehaviour
{
    public static Sg_ScoreManager Inst;

    [Header("Score")]
    [SerializeField] private int currentScore = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreTextObj;

    [Header("Popup")]
    [SerializeField] private ScorePopupNotifier scorePopupNotifier;

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
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
        if (amount < 0)
        {
            amount = 0;
        }

        currentScore += amount;
        UpdateScoreUI();

        if (scorePopupNotifier != null)
        {
            scorePopupNotifier.ShowScoreGain(amount);
        }

        Debug.Log($"[Sg_ScoreManager] AddScore: +{amount}, CurrentScore: {currentScore}");
    }

    public void RemoveScore(int amount)
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

    public void UpdateScoreUI()
    {
        if (scoreTextObj != null)
        {
            scoreTextObj.text = currentScore.ToString();
        }
    }
}