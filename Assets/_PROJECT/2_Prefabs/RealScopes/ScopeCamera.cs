using UnityEngine;

public class ScopeCamera : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = Camera.main.fieldOfView;
    }

    void Update()
    {
        cam.fieldOfView = Camera.main.fieldOfView;
    }
}
