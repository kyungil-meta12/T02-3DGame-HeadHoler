using UnityEngine;
using UnityEngine.InputSystem;

// 레이 피격 및 레그돌 활성화를 담당하는 모듈

public class RagdollController : MonoBehaviour
{
    public Collider[] ragdollColliders; // 레그돌 콜라이더
    public Rigidbody[] ragdollBodies; // 레그돌 리지드 바디
    public Collider headCollider; // 헤드샷 구분용 콜라이더
    public bool devMode = false; // 활성화 시 spaceKey로 레그돌 활성화 가능

    private bool ragdollEnabled = false;

    private Animator anim;
    [HideInInspector]
    public Collider psCollider; // 물리 시뮬레이션용 콜라이더
    private Rigidbody psRigidBody; // 물리 시뮬레이션용 리지드 바디

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animator>();
        psCollider = GetComponent<Collider>();
        psRigidBody = GetComponent<Rigidbody>();

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
    }
}
