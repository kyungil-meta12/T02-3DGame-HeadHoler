using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponCardHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private WeaponCardPreviewRotator previewRotator;
    [SerializeField] private GameObject hoverOutline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewRotator != null)
            previewRotator.SetHover(true);

        if (hoverOutline != null)
            hoverOutline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previewRotator != null)
            previewRotator.SetHover(false);

        if (hoverOutline != null)
            hoverOutline.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} 선택됨");
    }
}