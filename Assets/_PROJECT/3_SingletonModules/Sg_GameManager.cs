using System;
using System.Collections.Generic;
using UnityEngine;

public class Sg_GameManager : MonoBehaviour
{
    public static Sg_GameManager Inst;
    public List<Entity> entities;

    [Header("Result Popup")]
    [SerializeField] private ResultPopupController resultPopupController;

    private bool isGameOver = false;

    private void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        entities = new();
        print("[Sg_GameManager] Created instance.");
        entities = new List<Entity>();
    }

    private void Update()
    {
        if (isGameOver)
            return;

        if (Sg_ScoreManager.Inst == null)
            return;

        if (Sg_ScoreManager.Inst.CurrentScore <= 0)
        {
            GameOver();
        }
    }

    void OnDestroy()
    {
        Inst = null;
    }
    
    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        print("Game Over.");

        if (resultPopupController != null)
        {
            resultPopupController.ShowFailure();
        }
        else
        {
            Debug.LogWarning("[Sg_GameManager] resultPopupController is null.");
        }
    }
}
