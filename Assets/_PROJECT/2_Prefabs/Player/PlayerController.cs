using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float moveAcc;

    private Rigidbody body;
    private Vector3 moveDir;
    private Vector3 currDir;
    private Vector3 currDirDest;

    public Transform spine;

    private bool inputForward;
    private bool inputBackward;
    private bool inputStrafeLeft;
    private bool inputStrafeRight;

    private Animator anim;
    private SkinnedMeshRenderer smr;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        smr.updateWhenOffscreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        inputForward     = Input.GetKey(KeyCode.W);
        inputBackward    = Input.GetKey(KeyCode.S);
        inputStrafeLeft  = Input.GetKey(KeyCode.A);
        inputStrafeRight = Input.GetKey(KeyCode.D);

        currDirDest.z = 0f;
        if(inputForward)
        {
            currDirDest.z += 1f;
        }
        if(inputBackward)
        {
            currDirDest.z -= 1f;
        }

        currDirDest.x = 0f;
        if(inputStrafeRight)
        {
            currDirDest.x += 1f;
        }
        if(inputStrafeLeft)
        {
            currDirDest.x -= 1f;
        }

        anim.SetFloat("ForwardSpeed", moveDir.z);
        anim.SetFloat("StrafeSpeed", moveDir.x);
    }

    void FixedUpdate()
    {
        currDir = Vector3.Lerp(currDir, currDirDest, Time.fixedDeltaTime * moveAcc);
        moveDir = Vector3.ClampMagnitude(currDir, 1f);
        body.rotation = Quaternion.Euler(new Vector3(0f, Sg_MouseMan.Inst.rotation.y, 0f));
        var prevVel = body.linearVelocity;
        body.AddRelativeForce(moveDir * moveSpeed, ForceMode.VelocityChange);
        var currVel = body.linearVelocity;
        body.linearVelocity = new Vector3(currVel.x, prevVel.y, currVel.z);
    }

    void LateUpdate()
    {
        Vector3 worldAxis = body.transform.TransformDirection(Vector3.right);
        Quaternion rotationDelta = Quaternion.AngleAxis(Sg_MouseMan.Inst.rotation.x, worldAxis);
        spine.rotation = rotationDelta * spine.rotation;
    }
}
