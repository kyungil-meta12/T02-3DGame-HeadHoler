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
    [Header("소속")]
    public Team myTeam;
    public Role myRole;
    [Header("최개 체력")]
    public float maxHP = 100f;
    [Header("성별 선택")]
    public bool isMale = true;
    [Header("랜더러 선택")]
    public int rendererIndex = 0;
    [Header("머리장비 선택")]
    public int equipIndex = 99;
    [Header("순찰 포인트")]
    public List<GameObject> patrolPoints;
    [Header("경계 지속시간")]
    public static float alertTimer = 5f;

    private static readonly int Speed = Animator.StringToHash("Speed");

    [Space(20)] [Header("======참조======")] 
    [Header("남성 랜더러")]
    public Mesh[] maleRenderMeshes; 
    [Header("여성 랜더러")]
    public Mesh[] femaleRenderMeshes; 
    [Header("머리장비")]
    public GameObject[] equips;
    [Header("애니메이터 컨트롤러")]
    public AnimatorController[] animatorControllers;

    internal int alertLevel;
    
    private Animator animator;
    private SkinnedMeshRenderer rend;
    private BehaviorGraphAgent behaviorGraphAgent;
    private RagdollController ragdollController;
    private NavMeshAgent agent;
    private float speed;
    private Vector3 postPos;

    internal float currentHP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        ragdollController = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Sg_GameManager.Inst.entities.Add(this);
        behaviorGraphAgent.Init();
        
        //소속, 순찰포인트 세팅
        behaviorGraphAgent.SetVariableValue("Role", myRole);
        behaviorGraphAgent.SetVariableValue("PatrolPoints", patrolPoints);
        currentHP = maxHP;

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
            Instantiate(equips[equipIndex], ragdollController.headCollider.transform);
        }
        
        //if(alertRoutine == null) alertRoutine = StartCoroutine(AlertTimer());
        postPos = transform.position;
    }

    private void Update()
    {
        //속도
        
        
        if (!ragdollController.ragdollEnabled)
        {
            float speed = agent.desiredVelocity.magnitude;
            if (speed > 1f) speed = 1f;
            animator.SetFloat(Speed, speed, 0.1f, Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        if (Sg_GameManager.Inst.entities.Contains(this))
        {
            Sg_GameManager.Inst.entities.Remove(this);
        }
    }
    
    //경계 루틴 : 일정시간 이후 경계 풀림
    private Coroutine alertRoutine = null;
    private WaitForSeconds alertWait = new WaitForSeconds(alertTimer);
    private IEnumerator AlertTimer()
    {
        if(ragdollController.ragdollEnabled) yield break;
        alertLevel += 1;
        animator.SetBool(isAlert, alertLevel > 0);
        while (alertLevel > 0)
        {
            yield return alertWait;
            if(ragdollController.ragdollEnabled) yield break;
            alertLevel -= 1;
            animator.SetBool(isAlert, alertLevel > 0);
        }
    }

    public void Hit(RaycastHit hit, Vector3 direction, float dmg)
    {
        var behavior = GetComponent<BehaviorGraphAgent>();
        var regController = GetComponent<RagdollController>();
        if (behavior.GetVariable<bool>("isHurt", out var isHurt))
        {
            isHurt.Value = true;
            if (hit.collider == regController.headCollider)
            {
                currentHP = 0f;
                behavior.enabled = false;
                regController.EnableRagdoll();
                regController.headCollider.attachedRigidbody.AddForce(direction * 200f, ForceMode.Impulse); // 맞은 방향으로 힘 가함
                print("headshot");
            }
            else
            {
                foreach (var c in regController.ragdollColliders)
                {
                    if (hit.collider == c)
                    {
                        currentHP -= dmg;
                        currentHP = Mathf.Clamp(currentHP, 0f, 999f);
                        if (currentHP <= 0f)
                        {
                            behavior.enabled = false;
                            regController.EnableRagdoll();
                            c.attachedRigidbody.AddForce(direction * 200f, ForceMode.Impulse); // 죽은 이후에는 맞은 방향으로 힘 가함
                        }
                        print("not headshot");
                        print($"current HP: {currentHP}");
                        break;
                    }
                }
            }
        }
    }
}
