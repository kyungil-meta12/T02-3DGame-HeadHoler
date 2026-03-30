using UnityEngine;
using Michsky.UI.Heat;

public class AmmoProgressBarBinder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ProgressBar progressBar;

    [Header("Update Settings")]
    [SerializeField] private bool updateEveryFrame = true;

    private GunController currentGun;
    private int lastCurrAmmo = -1;
    private int lastTotalAmmo = -1;

    private void Start()
    {
        ResolveCurrentGun(forceRefresh: true);
    }

    private void Update()
    {
        if (!updateEveryFrame)
            return;

        if (playerController == null || progressBar == null)
            return;

        ResolveCurrentGun(forceRefresh: false);

        if (currentGun == null)
            return;

        if (lastCurrAmmo != currentGun.currAmmo || lastTotalAmmo != currentGun.totalAmmo)
        {
            RefreshUI();
        }
    }

    private void ResolveCurrentGun(bool forceRefresh)
    {
        GunController newGun = playerController != null ? playerController.CurrentGun : null;

        if (newGun != currentGun)
        {
            currentGun = newGun;
            lastCurrAmmo = -1;
            lastTotalAmmo = -1;

            if (forceRefresh || currentGun != null)
                RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (currentGun == null || progressBar == null)
            return;

        progressBar.maxValue = currentGun.totalAmmo;
        progressBar.currentValue = currentGun.currAmmo;
        progressBar.UpdateUI();

        lastCurrAmmo = currentGun.currAmmo;
        lastTotalAmmo = currentGun.totalAmmo;
    }
}