using UnityEngine;

public class PauseTargetSlotUI : MonoBehaviour
{
    [Header("Target Reference")]
    [SerializeField] private RagdollController target;

    [Header("UI")]
    [SerializeField] private GameObject clearedMarkObject;

    private void Awake()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (clearedMarkObject == null)
            return;

        bool isCleared = target != null && target.IsDead;
        clearedMarkObject.SetActive(isCleared);
    }
}