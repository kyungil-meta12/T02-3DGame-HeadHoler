using SETUtil.Extend;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScopeImageMove : MonoBehaviour
{
    private Vector2 mouseDelta;
    public RectTransform[] rt;
    private Vector2[] originPos;

    public Image scopeImage;
    public Sprite[] images;

    void Start()
    {
        originPos = new Vector2[rt.Length];
        for(int i = 0; i < rt.Length; i++)
        {
            originPos[i] = rt[i].localPosition;
        }

        // 총마다 다른 스코프 이미지를 선택한다
         scopeImage.sprite = images[Sg_GunIndex.Inst.GetCurrentIndex()];
    }

    void Update()
    {
        var delta = Mouse.current.delta.ReadValue();
        mouseDelta.x -= delta.x * 0.5f;
        mouseDelta.y -= delta.y * 0.5f;
        mouseDelta = Vector2.Lerp(mouseDelta, Vector2.zero, Time.deltaTime * 5f);
        for(int i = 0; i < rt.Length; i++)
        {
            rt[i].localPosition = originPos[i] + mouseDelta;
        }
    }
}
