using UnityEngine;

public class WheelController : MonoBehaviour
{
    public bool isRightWheel = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DestroyWheel(Vector3 direction)
    {
        var root = transform.root;
        root.GetComponent<CarController>().SetCarDamaged();
        rb.AddForce(isRightWheel ? -transform.right * 250f : transform.right * 250f, ForceMode.Impulse);
        var joint = GetComponent<ConfigurableJoint>();
        if (joint)
        {
            Destroy(joint);
            joint.connectedBody = null;
        }
        Sg_SfxPlayer.Inst.PlayWheelBreak();
    }
}
