using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float moveAcc;
    public Transform spine;

    private Rigidbody body;
    private Vector3 moveDir;
    private Vector3 currDir;
    private Vector3 currDirDest;



    private bool inputForward;
    private bool inputBackward;
    private bool inputStrafeLeft;
    private bool inputStrafeRight;

    private Animator anim;
    private SkinnedMeshRenderer smr;

    // 숨참기 값
    private bool holdState = false;
    private float holdTime;

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

        // 줌 상태에서 움직이면 해제된다.
        if (inputForward || inputBackward || inputStrafeLeft || inputStrafeRight)
        {
            Sg_CameraController.Inst.zoomState = false;
            Sg_CameraController.Inst.acc = 0f;
        }

        // 스페이스바를 눌러 숨 참기를 토글한다.
        // 줌을 사용하는 동안에만 가능하다.
        if(Sg_CameraController.Inst.zoomState) {
            if(Keyboard.current.spaceKey.wasPressedThisFrame) {
                if(holdState)
                {
                    holdState = false;
                }
                else
                {
                    if(holdTime <= 0f)
                    {
                        holdState = true;
                    }
                }
            }
        }
        else
        {
            holdState = false;
        }

        if(holdState)
        {
            holdTime += Time.deltaTime; // 숨 참기를 시작한 지 5초가 지나면 강제 해제
            if(holdTime >= 5f)
            {
                holdTime = 5f;
                holdState = false;
            }
        }
        else
        {
            holdTime -= Time.deltaTime; // 이전에 실행한 숨 참기 시간동안 숨 참기를 실행할 수 없다.
            if(holdTime < 0f)
            {
                holdTime = 0f;
            }
            holdState = false;
        }

        // 애니메이션 속도를 낮추어 숨을 참는 모션 표현
        anim.speed = Mathf.Lerp(anim.speed, holdState ? 0f : 1f, Time.deltaTime * 2.5f);
    }

    void FixedUpdate()
    {
        currDir = Vector3.Lerp(currDir, currDirDest, Time.fixedDeltaTime * moveAcc);
        moveDir = Vector3.ClampMagnitude(currDir, 1f);
        body.rotation = Quaternion.Euler(new Vector3(0f, Sg_MouseMan.Inst.rotation.y, 0f));
        body.AddRelativeForce(moveDir * moveSpeed, ForceMode.Force);
    }

    void LateUpdate()
    {
        Vector3 worldAxis = body.transform.TransformDirection(Vector3.right);
        Quaternion rotationDelta = Quaternion.AngleAxis(Sg_MouseMan.Inst.rotation.x, worldAxis);
        spine.rotation = rotationDelta * spine.rotation;
    }
}
