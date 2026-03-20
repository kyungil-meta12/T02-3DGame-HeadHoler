using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public float defaultFov;
    public float zoomedFov;
    public float zoomAcc;
    public float unzoomSpeed;
    public Vector2 zoomedSensitivityRatio;
    public Transform trackTarget;
    public Transform yRotationTarget;
    public Canvas canvas;

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

    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            zoomState = !zoomState;
            acc = 0f;

            if (zoomState) // 줌 활성화 시 마우스 감도를 더 낮게 설정한다
            {
                Sg_MouseMan.Inst.SetSensitivityMultiple(zoomedSensitivityRatio);
            }
            else // 줌 비활성화 시 마우스 감도를 다시 리셋
            {
                Sg_MouseMan.Inst.ResetSensitivityMultiple();
            }
        }
    }

    void LateUpdate()
    {
        var xRotation = Sg_MouseMan.Inst.rotation.x;
        var yRotation = yRotationTarget.rotation.eulerAngles.y;
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        cam.transform.position = trackTarget.position;

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
                canvas.gameObject.SetActive(false); // 일정 수치 이상으로 fov가 올라가면 스나이퍼 스코프 캔버스 비활성화
            }
        }

        cam.fieldOfView = currentFov;
    }
}
