using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sg_MouseMan : MonoBehaviour
{
    public static Sg_MouseMan Inst;


    public bool startWithLocked;

    public Vector2 sensitivity;


    private Vector2 defaultSensitivity;
    private Vector2 currentSensitivity;


    [HideInInspector]
    public Vector3 rotation = Vector3.zero;

    [HideInInspector]
    public bool lockState = false;

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
        defaultSensitivity = sensitivity;
        currentSensitivity = defaultSensitivity;

        print("[Sg_MouseController] Created instance.");
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
            var mouseDelta = Mouse.current.delta.ReadValue();
            rotation.x -= mouseDelta.y * currentSensitivity.x;
            rotation.y += mouseDelta.x * currentSensitivity.y;
            rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);
            rotation.y %= 360f;
            if (rotation.y < 0)
            {
                rotation.y += 360f;
            }
        }
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

    public void SetSensitivity(Vector2 val)
    {
        currentSensitivity = val;
    }

    public void ResetSensitivity()
    {
        currentSensitivity = defaultSensitivity;
    }
}
