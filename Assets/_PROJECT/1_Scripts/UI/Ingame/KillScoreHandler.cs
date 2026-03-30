using UnityEngine;

public class KillScoreHandler : MonoBehaviour
{
    public enum UnitType
    {
        Enemy,
        Ally
    }

    [Header("Unit Type")]
    [SerializeField] private UnitType unitType = UnitType.Enemy;

    [Header("Score Settings")]
    [SerializeField] private int enemyKillScore = 100;
    [SerializeField] private int allyDeathPenalty = 50;

    private bool isDead = false;

    public void OnDeath()
    {
        if (isDead == true)
        {
            return;
        }

        isDead = true;

        if (Sg_ScoreManager.Inst == null)
        {
            return;
        }

        if (unitType == UnitType.Enemy)
        {
            Sg_ScoreManager.Inst.AddScore(enemyKillScore);
        }
        else if (unitType == UnitType.Ally)
        {
            Sg_ScoreManager.Inst.RemoveScore(allyDeathPenalty);
        }
    }
}