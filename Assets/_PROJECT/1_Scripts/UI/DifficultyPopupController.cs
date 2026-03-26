using UnityEngine;
using Michsky.UI.Heat;

public class DifficultyPopupController : MonoBehaviour
{
    [Header("Popup Root")]
    [SerializeField] private GameObject popupRoot;

    [Header("Heat UI Panel Manager")]
    [SerializeField] private PanelManager panelManager;

    [Header("Panel Index")]
    [SerializeField] private int easyIndex = 0;
    [SerializeField] private int normalIndex = 1;
    [SerializeField] private int hardIndex = 2;

    public void OpenEasy()
    {
        OpenPopup();
        panelManager.OpenPanelByIndex(easyIndex);
    }

    public void OpenNormal()
    {
        OpenPopup();
        panelManager.OpenPanelByIndex(normalIndex);
    }

    public void OpenHard()
    {
        OpenPopup();
        panelManager.OpenPanelByIndex(hardIndex);
    }

    public void OpenByIndex(int index)
    {
        OpenPopup();
        panelManager.OpenPanelByIndex(index);
    }

    public void OpenPopup()
    {
        if (popupRoot != null)
            popupRoot.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);
    }
}