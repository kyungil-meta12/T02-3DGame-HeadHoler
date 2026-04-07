using UnityEngine;
using UnityEngine.UI;

public class Sg_ScopeZoomIndicator : MonoBehaviour
{
    public static Sg_ScopeZoomIndicator Inst;

    private float destValue;
    private Slider slider;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        slider = GetComponentInChildren<Slider>();
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, destValue, Time.deltaTime * 5f);
    }

    public void SetMaxCount(int count)
    {
        slider.maxValue = count;
    }

    // 현재 줌 카운트를 입력하면 슬라이더가 움직일 옥표 값이 변경 된다.
    public void InputZoomCount(int count)
    {
        destValue = (float)count;
    }
}
