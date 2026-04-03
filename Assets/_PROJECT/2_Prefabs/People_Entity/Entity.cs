using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Unity.Behavior;
using System.Linq;
using UnityEngine.AI;


public class Entity : MonoBehaviour
{
    private static readonly int isAlert = Animator.StringToHash("isAlert");
    [Header("======세팅======")]
    [Header("**PatrolPoint 씬에 배치하고 참조 필수**")]
    [Header("**시작 전 우클릭 -> 랜더러 및 머리장비 세팅 필수**")]
    [Header("소속")]
    public Team myTeam;
    public Role myRole;
    [Header("최대 체력")]
    public float maxHP = 100f;
    [Header("현재 체력")]
    public float currentHP;
    [Header("성별 선택")]
    public bool isMale = true;
    [Header("랜더러 선택")]
    public int rendererIndex = 0;
    [Header("머리장비 선택")]
    public int equipIndex = 99;
    [Header("가드 대상 (없으면 비워두기 가능)")]
    public GameObject guardTarget;
    [Header("순찰 포인트")]
    public List<GameObject> patrolPoints;

    private static readonly int Speed = Animator.StringToHash("Speed");

    [Space(20)] [Header("======참조======")] 
    [Header("남성 랜더러")]
    public Mesh[] maleRenderMeshes; 
    [Header("여성 랜더러")]
    public Mesh[] femaleRenderMeshes;
    [Header("머리 장비 슬롯")]
    public Transform headSlot;
    [Header("머리장비")]
    public GameObject[] equips;
    [Header("애니메이터 컨트롤러")]
    public AnimatorController[] animatorControllers;
    
    private Animator animator;
    private SkinnedMeshRenderer rend;
    private BehaviorGraphAgent behavior;
    private RagdollController regController;
    private NavMeshAgent agent;
    private float speed;

    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        behavior = GetComponent<BehaviorGraphAgent>();
        regController = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Sg_GameManager.Inst.entities.Add(this);
        behavior.Init();
        
        //소속, 순찰포인트 세팅
        behavior.SetVariableValue("Role", myRole);
        behavior.SetVariableValue("PatrolPoints", patrolPoints);
        behavior.SetVariableValue("GuardTarget", guardTarget);
        currentHP = maxHP;
    }

    [ContextMenu("랜더러 및 머리장비 세팅")]
    public void MakeUp()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        behavior = GetComponent<BehaviorGraphAgent>();
        regController = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
        
        //성별, 인덱스에 맞춰 랜더러 바꾸기
        if (isMale)
        {
            animator.runtimeAnimatorController = animatorControllers[0];
            int curIndex = rendererIndex % maleRenderMeshes.Length;
            rend.sharedMesh = maleRenderMeshes[curIndex];
        }
        else
        {
            animator.runtimeAnimatorController = animatorControllers[1];
            int curIndex = rendererIndex % femaleRenderMeshes.Length;
            rend.sharedMesh = femaleRenderMeshes[curIndex];
        }

        //머리장비 바꾸기
        if (equipIndex < equips.Length)
        {
            for (int i = 0; i < headSlot.childCount; i++)
            {
                Destroy(headSlot.GetChild(i).gameObject);
            }
            Instantiate(equips[equipIndex], headSlot);
        }
    }

    private void Update()
    {
        //애니메이터 속도전달
        if (!regController.ragdollEnabled)
        {
            float speed = agent.desiredVelocity.magnitude;
            if (speed > 1f) speed = 1f;
            animator.SetFloat(Speed, speed, 0.1f, Time.deltaTime);
        }

        if (currentHP <= 0f && isDead == false)
        {
            isDead = true;
            currentHP = 0f;
            behavior.enabled = false;
            agent.isStopped = true;
            regController.EnableRagdoll();
        }
    }

    private void OnDisable()
    {
        if (Sg_GameManager.Inst != null && Sg_GameManager.Inst.entities.Contains(this))
        {
            Sg_GameManager.Inst.entities.Remove(this);
        }
    }

    
    
    //총이나 충돌체에 맞았을때
    public void Hit(Collider col, Vector3 direction, float dmg)
    {
        if (behavior.GetVariable<bool>("isHurt", out var isHurt))
        {
            isHurt.Value = true;
            if (col == regController.headCollider)
            {
                currentHP = 0f;
                regController.headCollider.attachedRigidbody.AddForce(direction * 200f, ForceMode.Impulse); // 맞은 방향으로 힘 가함
                //print("headshot");
            }
            else
            {
                foreach (var c in regController.ragdollColliders)
                {
                    if (col == c)
                    {
                        currentHP -= dmg;
                        currentHP = Mathf.Clamp(currentHP, 0f, 999f);
                        if (regController.ragdollEnabled)
                        {
                            c.attachedRigidbody.AddForce(direction * 200f, ForceMode.Impulse); // 죽은 이후에는 맞은 방향으로 힘 가함
                        }
                        //print("not headshot");
                        //print($"current HP: {currentHP}");
                        break;
                    }
                }
            }
        }
    }

    public void Die()
    {
        currentHP = 0f;
        behavior.enabled = false;
        agent.isStopped = true;
        regController.EnableRagdoll();
    }

    public void ScanComplete()
    {
        Debug.Log("시체 스캔 완료");
        Destroy(gameObject);
    }
}
