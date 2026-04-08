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
    [Header("대기만 할지 선택")]
    public bool isDontMove;
    [Header("대기중 대화모션 적용여부 선택")] 
    public bool isTalking;
    [Header("일정시간 후 이동해야 하는 좌표 (없으면 비워두기 가능)")]
    public float pointToMoveTime = 60;
    public GameObject pointToMove;
    [Header("차에 타야하는지 체크")]
    public bool isMustGetInCar;
    [Header("차 위치")]
    public Transform carDriverSeatPoint;
    [Header("가드 대상 (없으면 비워두기 가능)")]
    public GameObject guardTarget;
    [Header("순찰 포인트")]
    public List<GameObject> patrolPoints;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int isInCar = Animator.StringToHash("isInCar");

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
    public RuntimeAnimatorController[] animatorControllers;

    private Vector3 driveSeatPoint;
    
    private Animator animator;
    private SkinnedMeshRenderer rend;
    private BehaviorGraphAgent behavior;
    private RagdollController regController;
    private NavMeshAgent agent;
    private float speed;

    internal bool isDead = false;
    internal BlackboardVariable<bool> isHurt;
    internal BlackboardVariable<bool> shotTrigger;
    internal BlackboardVariable<bool> isTimeToMove;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        behavior = GetComponent<BehaviorGraphAgent>();
        regController = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private IEnumerator Start()
    {
        behavior.enabled = false;
        behavior.Init();
        if (isDontMove)
        {
            behavior.enabled = false;
            if (isTalking)
            {
                animator.SetBool("isTalk", true);
            }
        }
		else
		{
			behavior.enabled = true;
		}
        
        //소속, 순찰포인트 세팅
        behavior.SetVariableValue("Role", myRole);
        behavior.SetVariableValue("PatrolPoints", patrolPoints);
        behavior.SetVariableValue("GuardTarget", guardTarget);
        behavior.SetVariableValue("PointToMove", pointToMove);
        behavior.SetVariableValue("isMustGetInCar", isMustGetInCar);

        behavior.GetVariable<bool>("isHurt", out isHurt);
        behavior.GetVariable<bool>("ShotTrigger", out shotTrigger);
        behavior.GetVariable<bool>("isTimeToMove", out isTimeToMove);
            
        currentHP = maxHP;

        if (pointToMove != null)
        {
            pointToMoveWait = new WaitForSeconds(pointToMoveTime);
            StartCoroutine(PointToMoveCoRoutine());
        }

        yield return new WaitUntil(() => Sg_GameManager.Inst != null);
        Sg_GameManager.Inst.entities.Add(this);
    }

    //목적지가 있을시 실행됨
    private WaitForSeconds pointToMoveWait;
    private IEnumerator PointToMoveCoRoutine()
    {
        yield return pointToMoveWait;

        if (pointToMove != null)
        {
            behavior.enabled = true;
            isTimeToMove.Value = true;
        }

        while (true)
        {
            if (Vector3.Distance(pointToMove.transform.position, transform.position) < 3f)
            {
                break;
            }
            yield return null;
        }
        
        //차에 타야하는지
        if (isMustGetInCar)
        {
            behavior.enabled = false;
            agent.enabled = false;
            //애니메이터 bool값 전부 끄기
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(param.name, false);
                }
            }
            
            //차량 탑승처리 : transform이 차량 머리쪽을 본다, 오른쪽으로 애니메이션에 맞춰 이동, 차량에 transform 고정해서 같이 이동
            while (true)
            {
                //몸 방향 -> 차량 머리방향 회전
                float angleDiff = Quaternion.Angle(transform.rotation, carDriverSeatPoint.transform.rotation * Quaternion.Euler(0f, 90f, 0f));

                if (angleDiff < 0.1f) 
                {
                    transform.rotation = carDriverSeatPoint.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
                    break; 
                }

                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    carDriverSeatPoint.transform.rotation * Quaternion.Euler(0f, 90f, 0f), 
                    Time.deltaTime * 5f);

                yield return null;
            }
            animator.SetBool(isInCar, true);

            yield return new WaitForSeconds(5f);
            rend.enabled = false;

            CarController carController = carDriverSeatPoint.GetComponentInParent<CarController>();
            carController.isTimeToMove = true;
            
            while (true)
            {
                //차량 운전석으로 위치 고정
                transform.position = carDriverSeatPoint.position + new Vector3(0, -1f, -0.6f);
                if (carController != null)
                {
                    if (carController.fireStarted)
                    {
                        yield return new WaitForSeconds(3f);
                        // 터지기 직전에 다친 상태로 나오기
                        rend.enabled = true;
                        animator.SetBool(isInCar, false);
                        behavior.enabled = true;
                        agent.enabled = true;
                        Hit(regController.ragdollColliders[0],
                            carController.transform.position - transform.position,10);
                        behavior.SetVariableValue("isMustGetInCar", false);
                        isMustGetInCar = false;
                        break;
                    }
                }
                yield return null;
            }
            
            //차량 터지면 폭발에 의해 사망
        }
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

        for (int i = 0; i < headSlot.childCount; i++)
        {
            DestroyImmediate(headSlot.GetChild(i).gameObject);
        }
        
        //머리장비 바꾸기
        if (equipIndex < equips.Length)
        {
            equip = Instantiate(equips[equipIndex], headSlot);
            equip.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    private GameObject equip;

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
            StopAllCoroutines();
        }
    }

    private void OnDisable()
    {
        if (Sg_GameManager.Inst != null && Sg_GameManager.Inst.entities.Contains(this))
        {
            Sg_GameManager.Inst.entities.Remove(this);
        }
    }

    public void Hit(HitData hitData)
    {
        //sendMessage 호출용, 상대속도로 데미지 적용
        
        if (hitData.impactForce > 5f)
        {
            Hit(hitData.col, hitData.direction, hitData.impactForce);
        }
    }
    
    //총이나 충돌체에 맞았을때
    public void Hit(Collider col, Vector3 direction, float dmg)
    {
        shotTrigger.Value = !shotTrigger.Value;
        isHurt.Value = true;
        if (col == regController.headCollider)
        {
            currentHP = 0f;
            regController.headCollider.attachedRigidbody.AddForce(direction * 100f, ForceMode.Impulse); // 맞은 방향으로 힘 가함
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
                        c.attachedRigidbody.AddForce(direction * 100f, ForceMode.Impulse); // 죽은 이후에는 맞은 방향으로 힘 가함
                    }
                    //print("not headshot");
                    //print($"current HP: {currentHP}");
                    break;
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
