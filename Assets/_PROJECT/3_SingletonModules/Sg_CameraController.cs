using UnityEngine;
using UnityEngine.InputSystem;

public class Sg_CameraController : MonoBehaviour
{
    public static Sg_CameraController Inst;

    public Camera cam;

    public float defaultFov;
    public float zoomedFov;
    public float zoomAcc;
    public float unzoomSpeed;
    public Transform trackTarget;
    public Transform yRotationTarget;
    public Canvas canvas;
    public Canvas crosshairCanvas;

    private float currentFov;
    private float offsetFov; // 스코프 줌 조정 오프셋 값
    private float offsetFovDest;
    [HideInInspector]
    public bool zoomState = false;

    [HideInInspector]
    public float acc = 0f;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }

        cam.fieldOfView = defaultFov;
        currentFov = defaultFov;
        
        Inst = this;
        print("[Sg_CameraMove] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;        
    }

    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // 스코프 배율 값 업데이트
        offsetFov = Mathf.Lerp(offsetFov, offsetFovDest, Time.deltaTime * 5f);

        // 줌을 더 크게 할 수록 마우스 감도 감소
        var camSensitivity = (offsetFov + currentFov)/defaultFov;
        Sg_MouseMan.Inst.SetSensitivityMultiple(new Vector2(camSensitivity, camSensitivity));
    }

    void LateUpdate()
    {
        var xRotation = Sg_MouseMan.Inst.rotation.x;
        var yRotation = yRotationTarget.rotation.eulerAngles.y;
        cam.transform.position = trackTarget.position;
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);    

        if (zoomState) // 줌 활성화 시 가속을 사용하여 fov 감소
        {
            currentFov -= acc;
            if (currentFov > zoomedFov)
            {
                acc += Time.deltaTime * zoomAcc;
                if (currentFov < defaultFov * 0.5f)
                {
                    canvas.gameObject.SetActive(true); // 일정 수치 미만으로 fov가 내려가면 스나이퍼 스코프 캔버스 활성화
                }
            }
            else // 값이 zoomedFov 미만으로 작아지지 않도록 고정
            {
                currentFov = zoomedFov;
            }

            cam.fieldOfView = currentFov + offsetFov;
        }
        else // 줌 비활성화 시 lerp를 사용하여 fov 증가
        {
            var t = Time.deltaTime * unzoomSpeed;
            if (t > 1f) // 델타시간 spike에 의해 lerp가 과도하게 계산되지 않도록 t 제한
            {
                t = 1f;
            }
            currentFov = Mathf.Lerp(currentFov, defaultFov, t);
            if (currentFov > defaultFov * 0.5f)
            {
                var comp = canvas.gameObject.GetComponentInChildren<ScopeImageMove>();
                comp.ResetRecoil(); // 비활성화 하기 직전에 남아있는 반동 진동 제거
                canvas.gameObject.SetActive(false); // 일정 수치 이상으로 fov가 올라가면 스나이퍼 스코프 캔버스 비활성화
            }

            cam.fieldOfView = currentFov;
        }
    }

    public void ToggleZoom()
    {
        zoomState = !zoomState;
        acc = 0f;
        crosshairCanvas.gameObject.SetActive(!zoomState); // zoomState가 활성화되면 크로스 헤어 비활성화
    }

    public void IncreaseScopeMagnification()
    {
        offsetFovDest -= 1f;
        offsetFovDest = Mathf.Clamp(offsetFovDest, -9f, 0f);
    }

    public void ReduceScopeMagnification()
    {
        offsetFovDest += 1f;
        offsetFovDest = Mathf.Clamp(offsetFovDest, -9f, 0f);
    }

    public void DisableZoom()
    {
        zoomState = false;
        acc = 0f;
    }
}
