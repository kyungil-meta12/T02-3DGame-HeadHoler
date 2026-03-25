using TMPro;
using UnityEngine;

public class WeaponCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectText;
    [SerializeField] private GameObject selectedOutline;
    [SerializeField] private string weaponId;

    private bool isSelected;

    public string WeaponId => weaponId;

    private void Start()
    {
        SetSelected(false);
    }

    public void OnClickCard()
    {
        WeaponCardGroupUI group = GetComponentInParent<WeaponCardGroupUI>();
        if (group != null)
        {
            group.SelectCard(this);
        }
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (selectText != null)
            selectText.text = isSelected ? "EQUIPPED" : "SELECT";

        if (selectedOutline != null)
            selectedOutline.SetActive(isSelected);
    }
}