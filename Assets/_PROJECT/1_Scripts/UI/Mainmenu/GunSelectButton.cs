using UnityEngine;

public class GunSelectButton : MonoBehaviour
{
    [SerializeField] private int gunIndex;

    public void OnClickSelectGun()
    {
        if (Sg_GunIndex.Inst == null)
        {
            Debug.LogWarning("Sg_GunIndex.Inst is null");
            return;
        }

        Sg_GunIndex.Inst.SelectIndex(gunIndex);
    }
}