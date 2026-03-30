using UnityEngine;

public class ResultPopupController : MonoBehaviour
{
    [Header("Popup References")]
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;

    [Header("Cursor Settings")]
    [SerializeField] private CursorLockMode resultCursorState = CursorLockMode.None;
    [SerializeField] private CursorLockMode gameCursorState = CursorLockMode.Locked;
    [SerializeField] private CursorVisibility resultCursorVisibility = CursorVisibility.Visible;
    [SerializeField] private CursorVisibility gameCursorVisibility = CursorVisibility.Default;

    private bool isResultShown = false;

    public enum CursorVisibility
    {
        Default,
        Invisible,
        Visible
    }

    private void Awake()
    {
        HideAll();
    }

    /// <summary>
    /// 승리 팝업 표시
    /// </summary>
    public void ShowVictory()
    {
        if (isResultShown) return;

        isResultShown = true;
        HideAll();

        if (victoryPopup != null)
            victoryPopup.SetActive(true);

        ApplyResultCursor();
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 패배 팝업 표시
    /// </summary>
    public void ShowDefeat()
    {
        if (isResultShown) return;

        isResultShown = true;
        HideAll();

        if (defeatPopup != null)
            defeatPopup.SetActive(true);

        ApplyResultCursor();
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 모든 결과 팝업 숨김
    /// </summary>
    public void HideAll()
    {
        if (victoryPopup != null)
            victoryPopup.SetActive(false);

        if (defeatPopup != null)
            defeatPopup.SetActive(false);
    }

    /// <summary>
    /// 결과 팝업 닫고 게임 상태 복구
    /// </summary>
    public void CloseResult()
    {
        HideAll();
        isResultShown = false;

        ApplyGameCursor();
        Time.timeScale = 1f;
    }

    private void ApplyResultCursor()
    {
        Cursor.lockState = resultCursorState;

        if (resultCursorVisibility == CursorVisibility.Visible)
            Cursor.visible = true;
        else if (resultCursorVisibility == CursorVisibility.Invisible)
            Cursor.visible = false;
    }

    private void ApplyGameCursor()
    {
        Cursor.lockState = gameCursorState;

        if (gameCursorVisibility == CursorVisibility.Visible)
            Cursor.visible = true;
        else if (gameCursorVisibility == CursorVisibility.Invisible)
            Cursor.visible = false;
    }
}