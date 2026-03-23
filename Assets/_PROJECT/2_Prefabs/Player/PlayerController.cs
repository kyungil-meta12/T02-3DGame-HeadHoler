using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float moveAcc;
    public Transform spine;

    // 게임 씬 시작 시 선택할 인덱스 // 개발 테스트용 변수
    // 빌드 시에는 inspector에서 false로 두고 빌드 할 것
    public bool devMode = false;

    public TwoBoneIKConstraint tb;
    public RigBuilder rBuild;

    public GameObject[] guns;
    public Transform[] handList;
    public Transform[] hintList;
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

        handList = new Transform[guns.Length];
        hintList = new Transform[guns.Length];

        // hand transform과 hint transform을 미리 리스트에 저장해둔다.
        for(int i = 0; i < guns.Length; i ++)
        {
            handList[i] = guns[i].transform.Find("Hand");
            hintList[i] = guns[i].transform.Find("Hint");
        }

        SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
    }

    void SetGun(int index)
    {
         // 일단 모든 총기 오브젝트를 비활성화 시킨 후, 
         // 현재 선택된 인덱스에 해당하는 총기만 활성화 한다.
        foreach(var g in guns)
        {
            g.SetActive(false);
        }

        // 지정된 인덱스로 guns에 있는 총기 중 하나를 선택
        guns[index].SetActive(true);

        // 그리고 hand transform과 hint transform을 지정한다.
        tb.data.target = handList[index];
        tb.data.hint = hintList[index];

        // 마지막으로 리그 빌드 재빌드
        // 게임 플레이 도중에는 변경될 일이 없기 때문에 그냥 빌드를 여기서 한다.
        rBuild.Build();
    }

    // Update is called once per frame
    void Update()
    {
        if(devMode)
        {
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                Sg_GunIndex.Inst.SelectIndex(0);
                SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
            }
            else if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Sg_GunIndex.Inst.SelectIndex(1);
                SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                Sg_GunIndex.Inst.SelectIndex(2);
                SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                Sg_GunIndex.Inst.SelectIndex(3);
                SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                Sg_GunIndex.Inst.SelectIndex(4);
                SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
            }
        }

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
