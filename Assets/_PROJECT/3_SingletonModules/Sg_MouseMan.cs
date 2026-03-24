using UnityEngine;
using UnityEngine.InputSystem;

// 마우스 커서 및 감도를 담당하는 모듈
// 한 번 추가되면 씬이 변경되어도 인스턴스는 계속 유지된다.

public class Sg_MouseMan : MonoBehaviour
{
    public static Sg_MouseMan Inst;


    public bool startWithLocked;

    public Vector2 sensitivity;


    private Vector2 sensitivityMultiply = Vector2.one;
    private float originRotationX = 0f;

    [HideInInspector]
    public Vector3 rotation = Vector3.zero;
    private Vector2 prevRotation = Vector2.zero;

    [HideInInspector]
    public bool lockState = false;

    private float recoilOffset = 0f;

    void Start()
    {
        if (Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;

        if (startWithLocked)
        {
            LockCursor();
        }

        print("[Sg_MouseMan] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;
    }

    void Update()
    {
        if(Application.isEditor) // 개발 모드에서 Tab키를 누르면 커서 토글 가능
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (lockState)
                {
                    UnlockCursor();
                }
                else
                {
                    LockCursor();
                }
            }
        }

        if (lockState) // 잠금 상태에서만 마우스 델타 업데이트
        {
            prevRotation.x = rotation.x;
            prevRotation.y = rotation.y;

            var mouseDelta = Mouse.current.delta.ReadValue();
            originRotationX -= mouseDelta.y * sensitivity.x * sensitivityMultiply.x;
            originRotationX = Mathf.Clamp(originRotationX, -90f, 90f);
            rotation.y += mouseDelta.x * sensitivity.y * sensitivityMultiply.y;
            rotation.x = originRotationX + recoilOffset;
            rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);
            rotation.y %= 360f;
            if (rotation.y < 0)
            {
                rotation.y += 360f;
            }
        }

        recoilOffset = Mathf.Lerp(recoilOffset, 0f, Time.deltaTime * 5f);
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockState = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        lockState = false;
    }

    public void SetSensitivityMultiple(Vector2 val)
    {
        sensitivityMultiply = val;
    }

    public void ResetSensitivityMultiple()
    {
        sensitivityMultiply = Vector2.one;
    }

    // 반동 추가
    public void AddRecoil(float val)
    {
        recoilOffset -= val;
    }
}
