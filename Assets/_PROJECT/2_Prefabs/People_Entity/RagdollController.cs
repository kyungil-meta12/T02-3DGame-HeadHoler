using UnityEngine;
using UnityEngine.InputSystem;
using static RagdollController;

// 레이 피격 및 레그돌 활성화를 담당하는 모듈

public class RagdollController : MonoBehaviour
{
    public Collider[] ragdollColliders; // 레그돌 콜라이더
    public Rigidbody[] ragdollBodies; // 레그돌 리지드 바디
    public Collider headCollider; // 헤드샷 구분용 콜라이더
    public GameObject hip; //시체처리용 참조
    public bool devMode = false; // 활성화 시 spaceKey로 레그돌 활성화 가능

    public enum ScoreMode
    {
        None,
        Add,
        Remove
    }

    [Header("Score")]
    [SerializeField] private ScoreMode scoreMode = ScoreMode.Add;
    [SerializeField] private int scoreOnDeath = 100;
    [SerializeField] private string scoreLabelOnDeath = "Enemy Kill";

    [Header("Clear Target")]
    [SerializeField] private bool isClearTarget = true;

    public bool IsClearTarget => isClearTarget;
    public bool IsDead => ragdollEnabled;

    internal bool ragdollEnabled = false;

    private Animator anim;
    [HideInInspector]
   // public CapsuleCollider psCollider; // 물리 시뮬레이션용 콜라이더
    //private Rigidbody psRigidBody; // 물리 시뮬레이션용 리지드 바디
    private SkinnedMeshRenderer smr; // 스킨드메쉬렌더러
    private EntityView entityView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animator>();
        //psCollider = GetComponent<CapsuleCollider>();
        //psRigidBody = GetComponent<Rigidbody>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        entityView = GetComponentInChildren<EntityView>();
    }

    void Start()
    {
        foreach (var rb in ragdollBodies) // 레그돌 바디들은 중력을 비활성화 하고 트리거 콜라이더로 설정한다
        {
            rb.useGravity = false;
        }
        foreach (var rc in ragdollColliders)
        {
            rc.isTrigger = true;
        }
        if (isClearTarget && Sg_TargetTracker.Inst != null)
        {
            Sg_TargetTracker.Inst.RegisterTarget(this);
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
        //var currLnVel = psRigidBody.linearVelocity * 1.5f;
        //var currAnVel = psRigidBody.angularVelocity;

      //  psRigidBody.isKinematic = true;
       // psCollider.enabled = false;
        anim.enabled = false;
        entityView.enabled = false;
        
        //시체 태그로 변경
        hip.tag = "Evidence";
        Transform[] allChildren = hip.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if(child.CompareTag("Entity"))
            {
                child.gameObject.tag = "Evidence";
            }
        }

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
           // rb.linearVelocity = currLnVel;
           // rb.angularVelocity = currAnVel;
            rb.linearDamping = 0.1f;
        }

        foreach (var rc in ragdollColliders)
        {
            rc.isTrigger = false;
        }

        Physics.SyncTransforms();

        ragdollEnabled = true;

        ApplyScore();

        if (isClearTarget && Sg_TargetTracker.Inst != null)
        {
            Sg_TargetTracker.Inst.NotifyTargetKilled(this);
        }

        // 렌더링 옵션 변경을 코루틴으로 한 프레임 미룸
        StartCoroutine(EnableOffscreenUpdateNextFrame());
    }
    private void ApplyScore()
    {
        if (Sg_ScoreManager.Inst == null || scoreOnDeath < 0)
            return;

        switch (scoreMode)
        {
            case ScoreMode.Add:
                Sg_ScoreManager.Inst.AddScore(scoreOnDeath, scoreLabelOnDeath);
                break;

            case ScoreMode.Remove:
                Sg_ScoreManager.Inst.RemoveScore(scoreOnDeath, scoreLabelOnDeath);
                break;
        }
    }

    private System.Collections.IEnumerator EnableOffscreenUpdateNextFrame()
    {
        // 물리 연산이 한 번 실행된 뒤에 렌더링 옵션 활성화
        yield return new WaitForFixedUpdate();
        smr.updateWhenOffscreen = true;
    }
}
