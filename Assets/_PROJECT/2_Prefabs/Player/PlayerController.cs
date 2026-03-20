using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float moveAcc;

    private Rigidbody body;
    private Vector3 moveDir;
    private Vector3 currDir;
    private Vector3 currDirDest;

    private bool inputForward;
    private bool inputBackward;
    private bool inputStrafeLeft;
    private bool inputStrafeRight;

    private Animator anim;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputForward     = Input.GetKey(KeyCode.W);
        inputBackward    = Input.GetKey(KeyCode.S);
        inputStrafeLeft  = Input.GetKey(KeyCode.A);
        inputStrafeRight = Input.GetKey(KeyCode.D);
    }

    void FixedUpdate()
    {
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

        currDir = Vector3.Lerp(currDir, currDirDest, Time.fixedDeltaTime * moveAcc);
        moveDir = Vector3.ClampMagnitude(currDir, 1f);
        body.rotation = Quaternion.Euler(new Vector3(0f, Sg_MouseMan.Inst.rotation.y, 0f));
        var currentVel = body.linearVelocity;
        body.AddRelativeForce(moveDir * moveSpeed, ForceMode.Acceleration);
        body.linearVelocity = currentVel;
    }
}
