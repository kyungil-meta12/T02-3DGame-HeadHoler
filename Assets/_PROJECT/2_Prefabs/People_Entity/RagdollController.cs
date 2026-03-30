using UnityEngine;
using UnityEngine.InputSystem;

// 레이 피격 및 레그돌 활성화를 담당하는 모듈

public class RagdollController : MonoBehaviour
{
    public Collider[] ragdollColliders; // 레그돌 콜라이더
    public Rigidbody[] ragdollBodies; // 레그돌 리지드 바디
    public SphereCollider headCollider; // 헤드샷 구분용 콜라이더
    public bool devMode = false; // 활성화 시 spaceKey로 레그돌 활성화 가능

    [Header("Score")]
    [SerializeField] private int scoreOnDeath = 100;
    [SerializeField] private bool giveScoreOnDeath = true;

    internal bool ragdollEnabled = false;

    private Animator anim;
    [HideInInspector]
    public CapsuleCollider psCollider; // 물리 시뮬레이션용 콜라이더
    private Rigidbody psRigidBody; // 물리 시뮬레이션용 리지드 바디
    private SkinnedMeshRenderer smr; // 스킨드메쉬렌더러

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animator>();
        psCollider = GetComponent<CapsuleCollider>();
        psRigidBody = GetComponent<Rigidbody>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        headCollider.radius = 0.2f; // 머리 콜라이더 조정

        foreach (var rb in ragdollBodies) // 레그돌 바디들은 중력을 비활성화 하고 트리거 콜라이더로 설정한다
        {
            rb.useGravity = false;
        }
        foreach (var rc in ragdollColliders)
        {
            rc.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ragdollEnabled && devMode && Keyboard.current.spaceKey.wasPressedThisFrame) // devMode 활성화 시 space를 누르면 레그돌 즉시 활성화
        {
            EnableRagdoll();
        }
    }

    public void EnableRagdoll()
    {
        var currLnVel = psRigidBody.linearVelocity * 1.5f;
        var currAnVel = psRigidBody.angularVelocity;

        psRigidBody.isKinematic = true;
        psCollider.enabled = false;
        anim.enabled = false;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = currLnVel;
            rb.angularVelocity = currAnVel;
            rb.linearDamping = 0.1f;
        }

        foreach (var rc in ragdollColliders)
        {
            rc.isTrigger = false;
        }

        Physics.SyncTransforms();

        ragdollEnabled = true;

        if (giveScoreOnDeath && Sg_ScoreManager.Inst != null)
        {
            Sg_ScoreManager.Inst.AddScore(scoreOnDeath);
        }

        // 렌더링 옵션 변경을 코루틴으로 한 프레임 미룸
        StartCoroutine(EnableOffscreenUpdateNextFrame());
    }

    private System.Collections.IEnumerator EnableOffscreenUpdateNextFrame()
    {
        // 물리 연산이 한 번 실행된 뒤에 렌더링 옵션 활성화
        yield return new WaitForFixedUpdate();
        smr.updateWhenOffscreen = true;
    }
}
