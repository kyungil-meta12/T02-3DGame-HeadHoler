using UnityEngine;
using UnityEngine.UI;

public class Sg_HitIndicator : MonoBehaviour
{
    public static Sg_HitIndicator Inst;
    Image img;
    float opacity = 0f;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        img = GetComponentInChildren<Image>();
        var imgColor = img.color;
        imgColor.a = 0f;
        img.color = imgColor;

        Inst = this;
        print("[HitIndicator] Instance created.");
    }

    private void OnDestroy()
    {
        Inst = null;
    }

    void Update()
    {
        opacity -= Time.deltaTime * 2f;
        opacity = Mathf.Clamp(opacity, 0f, 1f);
        var imgColor = img.color;
        imgColor.a = opacity;
        img.color = imgColor;
    }

    // 상호작용 가능한 물체를 맞추면 인디케이터를 활성화 한다
    public void InputHit()
    {
        opacity = 1f;
    }
}
