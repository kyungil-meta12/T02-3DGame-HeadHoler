using UnityEngine;

public class ResultPopupController : MonoBehaviour
{
    [System.Serializable]
    public class ScoreParticleSet
    {
        public string gradeName;
        public int minScore;
        public GameObject root;
        public ParticleSystem[] particles;
    }

    [Header("Popup Reference")]
    [SerializeField] private GameObject resultPopup;

    [Header("Result Images")]
    [SerializeField] private GameObject successImage;
    [SerializeField] private GameObject failureImage;

    [Header("Score Result UI")]
    [SerializeField] private UI_GameResultScoreList scoreListUI;

    [Header("Victory Particle By Score")]
    [SerializeField] private ScoreParticleSet[] victoryParticleSets;

    [Header("Defeat Particles")]
    [SerializeField] private GameObject defeatParticlesRoot;
    [SerializeField] private ParticleSystem[] defeatParticles;

    [Header("Cursor Settings")]
    [SerializeField] private CursorLockMode resultCursorState = CursorLockMode.None;
    [SerializeField] private CursorLockMode gameCursorState = CursorLockMode.Locked;
    [SerializeField] private CursorVisibility resultCursorVisibility = CursorVisibility.Visible;
    [SerializeField] private CursorVisibility gameCursorVisibility = CursorVisibility.Default;

    private bool isResultShown = false;
    public bool IsResultShown => isResultShown;

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

        if (scoreListUI != null)
            scoreListUI.Show();

        int finalScore = 0;
        if (Sg_ScoreManager.Inst != null)
            finalScore = Sg_ScoreManager.Inst.CurrentScore;

        if (Sg_AchievementRuntime.Inst != null)
        {
            Sg_AchievementRuntime.Inst.ProcessStageResult(finalScore, true);

            string stageId = Sg_AchievementRuntime.Inst.SelectedStageId;
            if (!string.IsNullOrEmpty(stageId))
            {
                Michsky.UI.Heat.ChapterManager.SetCompleted(stageId);                
            }
            PlayerPrefs.Save();
        }

        PlayVictoryParticlesByScore();

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

        if (scoreListUI != null)
            scoreListUI.Show();

        int finalScore = 0;
        if (Sg_ScoreManager.Inst != null)
            finalScore = Sg_ScoreManager.Inst.CurrentScore;

        if (Sg_AchievementRuntime.Inst != null)
            Sg_AchievementRuntime.Inst.ProcessStageResult(finalScore, false);

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

        HideVictoryParticleSets();

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

    private void PlayVictoryParticlesByScore()
    {
        HideVictoryParticleSets();

        int score = 0;

        if (Sg_ScoreManager.Inst != null)
            score = Sg_ScoreManager.Inst.CurrentScore;

        ScoreParticleSet selectedSet = null;

        if (victoryParticleSets != null)
        {
            for (int i = 0; i < victoryParticleSets.Length; i++)
            {
                ScoreParticleSet set = victoryParticleSets[i];

                if (set == null)
                    continue;

                if (score >= set.minScore)
                {
                    if (selectedSet == null || set.minScore > selectedSet.minScore)
                        selectedSet = set;
                }
            }
        }

        if (selectedSet == null)
        {
            Debug.LogWarning("[ResultPopupController] No matching victory particle set found.");
            return;
        }

        if (selectedSet.root != null)
            selectedSet.root.SetActive(true);

        if (selectedSet.particles != null)
        {
            foreach (var ps in selectedSet.particles)
            {
                if (ps == null)
                    continue;

                ps.gameObject.SetActive(true);
                ps.Play(true);
            }
        }

        Debug.Log($"[ResultPopupController] Victory Score: {score}, Selected Particle Grade: {selectedSet.gradeName}");
    }

    private void HideVictoryParticleSets()
    {
        if (victoryParticleSets == null)
            return;

        foreach (var set in victoryParticleSets)
        {
            if (set == null)
                continue;

            if (set.root != null)
                set.root.SetActive(false);
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
                if (ps == null)
                    continue;

                ps.gameObject.SetActive(true);
                ps.Play(true);
            }
        }
    }
}