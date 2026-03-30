using UnityEngine;

public class WeaponCardGroupUI : MonoBehaviour
{
    [SerializeField] private WeaponCardUI[] cards;

    private WeaponCardUI currentSelected;

    private void Awake()
    {
        if (cards == null || cards.Length == 0)
            cards = GetComponentsInChildren<WeaponCardUI>(true);
    }

    private void Start()
    {
        // 시작 시 첫 번째 카드 기본 선택
        if (cards != null && cards.Length > 0)
        {
            SelectCard(cards[0]);
        }
    }

    public void SelectCard(WeaponCardUI selectedCard)
    {
        if (selectedCard == null)
            return;

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;
            cards[i].SetSelected(cards[i] == selectedCard);
        }

        currentSelected = selectedCard;

        Debug.Log($"선택된 무기: {currentSelected.WeaponId}");
    }

    public WeaponCardUI GetCurrentSelected()
    {
        return currentSelected;
    }
}