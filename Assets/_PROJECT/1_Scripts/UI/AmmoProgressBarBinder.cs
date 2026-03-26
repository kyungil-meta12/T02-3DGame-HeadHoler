using UnityEngine;
using Michsky.UI.Heat;

public class AmmoProgressBarBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunController gunController;
    [SerializeField] private ProgressBar progressBar;

    [Header("Update Settings")]
    [SerializeField] private bool updateEveryFrame = true;

    private int lastCurrAmmo = -1;
    private int lastTotalAmmo = -1;

    private void Start()
    {
        RefreshUI();
    }
    private void Update()
    {
        if (updateEveryFrame == false)
            return;

        if (gunController == null || progressBar == null)
            return;

        if (lastCurrAmmo != gunController.currAmmo || lastTotalAmmo != gunController.totalAmmo)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (gunController == null || progressBar == null)
            return;

        progressBar.maxValue = gunController.totalAmmo;
        progressBar.currentValue = gunController.currAmmo;
        progressBar.UpdateUI();

        lastCurrAmmo = gunController.currAmmo;
        lastTotalAmmo = gunController.totalAmmo;
    }
}