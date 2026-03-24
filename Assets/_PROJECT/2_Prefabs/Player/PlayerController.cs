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
    private Transform[] handList;
    private Transform[] hintList;
    private GunController currGun;

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


    // 재장전 관리에 사용하는 AnimatorStateInfo
    private AnimatorStateInfo animInfo;
    private int upperLayerIndex;
    private bool onReloading = false;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        smr.updateWhenOffscreen = true;
    }

    void Start()
    {
        handList = new Transform[guns.Length];
        hintList = new Transform[guns.Length];

        // hand transform과 hint transform을 미리 리스트에 저장해둔다.
        for (int i = 0; i < guns.Length; i++)
        {
            handList[i] = guns[i].transform.Find("Hand");
            hintList[i] = guns[i].transform.Find("Hint");
        }

        SetGun(Sg_GunIndex.Inst.GetCurrentIndex());
        upperLayerIndex = anim.GetLayerIndex("Upper Layer");
    }

    void SetGun(int index)
    {
        // 일단 모든 총기 오브젝트를 비활성화 시킨 후, 
        // 현재 선택된 인덱스에 해당하는 총기만 활성화 한다.
        foreach (var g in guns)
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

        // 총기 컨트롤러 컴포넌트 찾기
        currGun = guns[index].GetComponent<GunController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (devMode)
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

        animInfo = anim.GetCurrentAnimatorStateInfo(upperLayerIndex);
        onReloading = animInfo.IsName("Reload");

        UpdateGun();
        UpdateMove();
        UpdateZoom();
        UpdateBreatheHold();
        UpdateReload();
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

    void UpdateGun()
    {
        bool triggerPulled = Mouse.current.leftButton.isPressed && !onReloading;
        currGun.SetGunTrigger(triggerPulled);
        if(Keyboard.current.rKey.wasPressedThisFrame && !onReloading)
        {
            currGun.ReloadGun();
        }
    }

    void UpdateMove()
    {
        inputForward = Input.GetKey(KeyCode.W);
        inputBackward = Input.GetKey(KeyCode.S);
        inputStrafeLeft = Input.GetKey(KeyCode.A);
        inputStrafeRight = Input.GetKey(KeyCode.D);

        currDirDest.z = 0f;
        if (inputForward)
        {
            currDirDest.z += 1f;
        }
        if (inputBackward)
        {
            currDirDest.z -= 1f;
        }

        currDirDest.x = 0f;
        if (inputStrafeRight)
        {
            currDirDest.x += 1f;
        }
        if (inputStrafeLeft)
        {
            currDirDest.x -= 1f;
        }

        anim.SetFloat("ForwardSpeed", moveDir.z);
        anim.SetFloat("StrafeSpeed", moveDir.x);
    }

    void UpdateZoom()
    {
        // 마우스 우클릭으로 줌 상태 토글
        if (!onReloading && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Sg_CameraController.Inst.ToggleZoom();
        }

        // 줌 상태에서 움직이거나 재장전을 실행하면 해제된다.
        if (onReloading || inputForward || inputBackward || inputStrafeLeft || inputStrafeRight)
        {
            Sg_CameraController.Inst.DisableZoom();
        }

        // 마우스 휠로 스코프 배율 조정
        var scroll = Mouse.current.scroll.ReadValue();
        if (!onReloading && scroll.y > 0f)
        {
            Sg_CameraController.Inst.IncreaseScopeMagnification();
        }
        else if (!onReloading && scroll.y < 0f)
        {
            Sg_CameraController.Inst.ReduceScopeMagnification();
        }
    }

    void UpdateBreatheHold()
    {
        // 스페이스바를 눌러 숨 참기를 토글한다.
        // 줌을 사용하는 동안에만 가능하다.
        if (Sg_CameraController.Inst.zoomState)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (holdState)
                {
                    holdState = false;
                }
                else
                {
                    if (holdTime <= 0f)
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

        if (holdState)
        {
            holdTime += Time.deltaTime; // 숨 참기를 시작한 지 5초가 지나면 강제 해제
            if (holdTime >= 5f)
            {
                holdTime = 5f;
                holdState = false;
            }
        }
        else
        {
            holdTime -= Time.deltaTime; // 이전에 실행한 숨 참기 시간동안 숨 참기를 실행할 수 없다.
            if (holdTime < 0f)
            {
                holdTime = 0f;
            }
            holdState = false;
        }

        // 숨을 참을 때는 애니메이션 속도를 낮추어 화면 흔들림 제거
        anim.speed = Mathf.Lerp(anim.speed, holdState ? 0f : 1f, Time.deltaTime * 2.5f);
    }

    void UpdateReload()
    {
        // 애니메이션이 재생 중이 아닐 때만 재장전 실행 가능
        if (!onReloading)
        {
            if(Keyboard.current.rKey.wasPressedThisFrame) { // 재장전 실행 시 ik weight를 0으로 설정
                anim.SetTrigger("Reload");
            }
        }
        
        // 2. 현재 상태가 "Reload"이거나, "Reload"로 전이 중인지 확인
        bool isActuallyReloading = anim.GetCurrentAnimatorStateInfo(upperLayerIndex).IsName("Reload") 
                                || anim.GetNextAnimatorStateInfo(upperLayerIndex).IsName("Reload");

        // 3. weight 값을 선형 보간(Lerp)으로 부드럽게 조절
        // 재장전 중이면 0 (IK 꺼짐), 아니면 1 (IK 켜짐)
        float targetWeight = isActuallyReloading ? 0f : 1f;
        tb.weight = Mathf.Lerp(tb.weight, targetWeight, Time.deltaTime * 10f);
    }
}
