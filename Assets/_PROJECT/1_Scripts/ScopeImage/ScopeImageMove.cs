using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScopeImageMove : MonoBehaviour
{
    private Vector2 mouseDelta;
    public RectTransform[] rt;
    private Vector2[] originPos;

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
