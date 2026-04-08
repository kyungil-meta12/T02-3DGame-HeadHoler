using System;
using System.Collections.Generic;
using UnityEngine;

public class Sg_GameManager : MonoBehaviour
{
    public static Sg_GameManager Inst;
    public List<Entity> entities = new();

    [Header("Result Popup")]
    [SerializeField] private ResultPopupController resultPopupController;

    private bool isGameOver = false;
    public bool isPaused = false;

    private void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        print("[Sg_GameManager] Created instance.");
    }

    private void Update()
    {
        if (isGameOver)
            return;

        if (Sg_ScoreManager.Inst == null)
            return;

        if (Sg_ScoreManager.Inst.CurrentScore <= 0)
        {
            SetGameOver();
        }
    }

    void OnDestroy()
    {
        Inst = null;
    }
    
    public void SetGameOver()
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

    // 일시정지 활성화
    public void SetPause()
    {
        Sg_MouseMan.Inst.UnlockCursor();
        isPaused = true;
    }

    // 일시정지 비활성화
    public void RevertPause()
    {
        Sg_MouseMan.Inst.LockCursor();
        isPaused = false;
    }
}
