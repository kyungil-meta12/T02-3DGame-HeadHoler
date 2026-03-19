using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public float defaultFov;
    public float zoomedFov;
    public float zoomAcc;
    public float unzoomSpeed;
    public Vector2 zoomedSensitivity;

    private float currentFov;
    private Camera cam;
    private bool zoomState = false;

    private float acc;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = defaultFov;
        currentFov = defaultFov;
    }

    void Update()
    {
        cam.transform.rotation = Quaternion.Euler(Sg_MouseMan.Inst.rotation);

        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            zoomState = !zoomState;
            acc = 0f;

            // 줌 활성화 시 마우스 감도를 더 낮게 설정한다
            if(zoomState)
            {
                Sg_MouseMan.Inst.SetSensitivity(zoomedSensitivity);
            }
            else
            {
                Sg_MouseMan.Inst.ResetSensitivity();
            }
        }

        if(zoomState) // 줌 활성화 시 가속을 사용하여 fov 감소
        {
            currentFov -= acc;
            if(currentFov > zoomedFov)
            {
                acc += Time.deltaTime * zoomAcc;
            }
            else // 값이 zoomedFov 미만으로 작아지지 않도록 고정
            {
                currentFov = zoomedFov;
            }
        }
        else // 줌 비활성화 시 lerp를 사용하여 fov 증가
        {
            var t = Time.deltaTime * unzoomSpeed;
            if(t > 1f) // 델타시간 spike에 의해 lerp가 과도하게 계산되지 않도록 t 제한
            {
                t = 1f;
            }
           currentFov = Mathf.Lerp(currentFov, defaultFov, t);
        }

        cam.fieldOfView = currentFov;
    }
}
