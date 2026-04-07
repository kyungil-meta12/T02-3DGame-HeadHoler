using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScopeImageMove : MonoBehaviour
{
    private Vector2 mouseDelta;
    public RectTransform[] rt;
    private Vector2[] originPos;
    private Vector2 shakeOffset;

    public Image scopeImage;

    private float shakeAmount;
    private float shakeTimer;
    public float shakeStrength;

    void Start()
    {
        originPos = new Vector2[rt.Length];
        for(int i = 0; i < rt.Length; i++)
        {
            originPos[i] = rt[i].localPosition;
        }
    }

    void Update()
    {
        if(!Sg_MouseMan.Inst.lockState) // 일시정지 상태에서는 업데이트 건너뜀
        {
            return;
        }

        var delta = Sg_MouseMan.Inst.delta;
        mouseDelta.x -= delta.x * 0.5f;
        mouseDelta.y -= delta.y * 0.5f;
        mouseDelta = Vector2.Lerp(mouseDelta, Vector2.zero, Time.deltaTime * 5f);

        shakeAmount = Mathf.Lerp(shakeAmount, 0f, Time.deltaTime * 5f);
        var shakeVal = shakeAmount * shakeStrength;
        shakeTimer += Time.deltaTime;
        if(shakeTimer > 0.02f)
        {
            shakeTimer -= 0.02f;
            shakeOffset.x = Random.Range(-shakeVal, shakeVal);
            shakeOffset.y = Random.Range(-shakeVal, shakeVal);
        }

        for(int i = 0; i < rt.Length; i++)
        {
            rt[i].localPosition = originPos[i] + mouseDelta + shakeOffset;
        }
    }

    public void AddRecoil(float val)
    {
        shakeAmount += val;
    }

    // 총기 격발 직후 남아있는 흔들림을 제거하는 메서드
    public void ResetRecoil()
    {
        for (int i = 0; i < rt.Length; i++)
        {
            rt[i].localPosition = originPos[i];
        }
        shakeAmount = 0f;
        mouseDelta = Vector2.zero;
    }
}
