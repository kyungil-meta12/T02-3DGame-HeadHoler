using UnityEngine;

public class WeaponCardPreviewRotator : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 20f;
    [SerializeField] private float hoverSpeed = 80f;

    private float currentSpeed;

    private void Awake()
    {
        currentSpeed = normalSpeed;
    }

    private void Update()
    {
        transform.Rotate(0f, currentSpeed * Time.unscaledDeltaTime, 0f, Space.World);
    }

    public void SetHover(bool isHover)
    {
        currentSpeed = isHover ? hoverSpeed : normalSpeed;
    }

    public void SetNormal()
    {
        currentSpeed = normalSpeed;
    }
}