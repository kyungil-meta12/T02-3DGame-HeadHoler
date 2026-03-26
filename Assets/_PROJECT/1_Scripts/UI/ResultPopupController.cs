using UnityEngine;

public class ResultPopupController : MonoBehaviour
{
    [Header("Popup References")]
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;

    private bool isResultShown = false;

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

        Time.timeScale = 0f; // 게임 멈춤
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

        Time.timeScale = 0f; // 게임 멈춤
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
}