using UnityEngine;

public class ResultPopupController : MonoBehaviour
{
    [Header("Popup Reference")]
    [SerializeField] private GameObject resultPopup;

    [Header("Result Images")]
    [SerializeField] private GameObject successImage;
    [SerializeField] private GameObject failureImage;

    [Header("Particle Roots")]
    [SerializeField] private GameObject victoryParticlesRoot;
    [SerializeField] private GameObject defeatParticlesRoot;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem[] victoryParticles;
    [SerializeField] private ParticleSystem[] defeatParticles;

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
    /// 성공 결과 팝업 표시
    /// </summary>
    public void ShowSuccess()
    {
        if (isResultShown)
            return;

        isResultShown = true;

        if (resultPopup != null)
            resultPopup.SetActive(true);

        if (successImage != null)
            successImage.SetActive(true);

        if (failureImage != null)
            failureImage.SetActive(false);

        PlayVictoryParticles();

        ApplyResultCursor();
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 실패 결과 팝업 표시
    /// </summary>
    public void ShowFailure()
    {
        if (isResultShown)
            return;

        isResultShown = true;

        if (resultPopup != null)
            resultPopup.SetActive(true);

        if (successImage != null)
            successImage.SetActive(false);

        if (failureImage != null)
            failureImage.SetActive(true);

        PlayDefeatParticles();

        ApplyResultCursor();
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 모든 결과 UI 숨김
    /// </summary>
    public void HideAll()
    {
        if (resultPopup != null)
            resultPopup.SetActive(false);

        if (successImage != null)
            successImage.SetActive(false);

        if (failureImage != null)
            failureImage.SetActive(false);

        if (victoryParticlesRoot != null)
            victoryParticlesRoot.SetActive(false);

        if (defeatParticlesRoot != null)
            defeatParticlesRoot.SetActive(false);
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

    private void PlayVictoryParticles()
    {
        if (victoryParticlesRoot != null)
            victoryParticlesRoot.SetActive(true);

        if (victoryParticles != null)
        {
            foreach (var ps in victoryParticles)
            {
                if (ps == null) continue;
                ps.gameObject.SetActive(true);
                ps.Play(true);
            }
        }
    }

    private void PlayDefeatParticles()
    {
        if (defeatParticlesRoot != null)
            defeatParticlesRoot.SetActive(true);

        if (defeatParticles != null)
        {
            foreach (var ps in defeatParticles)
            {
                if (ps == null) continue;
                ps.gameObject.SetActive(true);
                ps.Play(true);
            }
        }
    }
}