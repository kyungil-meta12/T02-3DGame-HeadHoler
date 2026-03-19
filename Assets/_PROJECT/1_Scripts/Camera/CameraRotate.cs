using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cam.transform.rotation = Quaternion.Euler(Sg_MouseMan.Inst.rotation);
    }
}
