using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//todo 즉사가 아닐때
//todo secondState로 경계, 소리지르기, 상처 관리

//하체 행동
public enum FirstState
{
    //대기, 걷기, 뛰기, 관찰, 저격위치 발각, 죽음,
    Idle, Walk, Run, See, Discover, Dead,
}

//상체 행동
public enum SecondState
{
    //없음, 경계, 소리지르기, 상처,
    None, Careful, Scream, Hurt,
}

public class Character : MonoBehaviour
{
    [Header("순찰 위치")]
    public Vector3[] destinationsPos; // 자동 이동 지정위치
    [Header("시야범위")]
    public Collider viewCol; //감지용 콜라이더
    [Header("시야각")]
    public float viewAngle = 90f; //색적범위
    [Header("관찰대상 레이어")]
    public LayerMask Evidence; //색적대상

    [Header(("각 행동별 지속시간"))] 
    public float idleTime = 1f;
    public float carefulTime = 1f;
    public float screamTime = 1f;
    public float hurtTime = 1f;
    
    private Animator anim; //애니메이터
    private NavMeshAgent agent; //AI
    
    private FirstState curFirstState; //현재 하체 행동상태
    private SecondState curSecondState; //현재 상체 행동상태
    private Array stateValues; //enum개수 체크용
    private Transform target; //관찰대상
    private bool isGotShot;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    protected virtual void Start()
    {
        stateValues = Enum.GetValues(typeof(FirstState));
        curFirstState = FirstState.Idle;

        StartCoroutine(MoveCoroutine());
    }

    #region 기본 코루틴
    protected virtual IEnumerator MoveCoroutine()
    {
        while (true)
        {
            switch (curFirstState)
            {
                case FirstState.Idle:
                    ChangeFirstAnim(FirstState.Idle);
                    yield return Idle();
                    break;
                case FirstState.Walk:
                    ChangeFirstAnim(FirstState.Walk);
                    yield return Walk();
                    break;
                case FirstState.Run:
                    ChangeFirstAnim(FirstState.Run);
                    yield return Run();
                    break;
                case FirstState.See:
                    ChangeFirstAnim(FirstState.See);
                    yield return See();
                    break;
                case FirstState.Discover:
                    ChangeFirstAnim(FirstState.Discover);
                    yield return Discover();
                    break;
                case FirstState.Dead:
                    ChangeFirstAnim(FirstState.Dead);
                    yield return Dead();
                    break;
            }

            yield return null;
        }
    }
    #endregion

    #region FirstState별 코루틴
    //대기
    protected virtual IEnumerator Idle()
    {
        //일정시간 후 걷기
        float time = 0f;
        while (time < 1)
        {
            if (curFirstState != FirstState.Idle)
            {
                StopCoroutine(Idle());
                break;
            }
            time += Time.deltaTime;
            float t = time / idleTime;
            yield return null;
        }
        curFirstState = FirstState.Walk;
    }
    
    //걷기
    protected virtual IEnumerator Walk()
    {
        //목표까지 걸어간 후 대기 or 걷는 중에 랜덤확률로 대기
        
        foreach (var dest in destinationsPos)
        {
            agent.SetDestination(dest);
            while (true)
            {
                if (curFirstState != FirstState.Walk)
                {
                    agent.isStopped = true;
                    StopCoroutine(Walk());
                    break;
                }
                if(Vector3.Distance(transform.position, dest) <= 1f) break;
                yield return null;
            }
        }
        
        agent.isStopped = true;
        curFirstState = FirstState.Idle;
    }

    //뛰기
    protected virtual IEnumerator Run()
    {
        //목표까지 뛰어간 후 대기
        
        foreach (var dest in destinationsPos)
        {
            agent.SetDestination(dest);
            while (true)
            {
                if (curFirstState != FirstState.Run)
                {
                    agent.isStopped = true;
                    StopCoroutine(Run());
                    break;
                }
                if(Vector3.Distance(transform.position, dest) <= 1f) break;
                yield return null;
            }
        }
        
        agent.isStopped = true;
        curFirstState = FirstState.Idle;
    }
    
    //관찰
    protected virtual IEnumerator See()
    {
        //관찰대상 사라질때까지 관찰 후 대기 or 특정 상황 관찰 후 대기
        while (target != null)
        {
            if(curFirstState != FirstState.See) break;
            transform.forward = target.position - transform.position;
            yield return null;
        }
        curFirstState = FirstState.Idle;
    }
    
    //저격위치 발각
    protected virtual IEnumerator Discover()
    {
        //todo 카메라 클로즈업, 발각 모션 후 게임 오버
        //transform.forward = 플레이어.position - transform.position;
        yield return null;
    }
    
    //죽음
    protected virtual IEnumerator Dead()
    {
        //행동 종료, 관찰대상 레이어로 바꾸기
        transform.gameObject.layer = Evidence;
        StopAllCoroutines();
        yield return null;
    }
    
    #endregion

    #region SecondState 행동별 코루틴
    //경계
    // protected virtual IEnumerator Careful()
    // {
    //     //일정시간 경계 후 대기
    //     float time = 0f;
    //     while (time < 1)
    //     {
    //         if (curFirstState != FirstState.Careful)
    //         {
    //             StopCoroutine(Careful());
    //             break;
    //         }
    //         time += Time.deltaTime;
    //         float t = time / carefulTime;
    //         yield return null;
    //     }
    //     curFirstState = FirstState.Idle;
    // }

    //소리지르기
    // protected virtual IEnumerator Scream()
    // {
    //     //일정 시간 소리지른 후 관찰
    //     if (target != null)
    //     {
    //         transform.forward = target.position - transform.position;
    //     }
    //     CallFriend();
    //     
    //     float time = 0f;
    //     while (time < 1)
    //     {
    //         if (curFirstState != FirstState.Scream)
    //         {
    //             StopCoroutine(Scream());
    //             break;
    //         }
    //         time += Time.deltaTime;
    //         float t = time / screamTime;
    //         yield return null;
    //     }
    //
    //     curFirstState = FirstState.See;
    // }

    //동료 부르기
    protected virtual void CallFriend()
    {
        //todo 범위 내 동료의 이동타겟 바꾸기,강제 이동
    }

    //상처
    protected virtual IEnumerator Hurt()
    {
        //자리에서 계속 소리지르기, 동료가 오면 일정시간 후 치료됨, 동료가 안오면 일정시간 후 과다출혈 사망
        float time = 0f;
        while (time < hurtTime)
        {
            time += Time.deltaTime;
            float t = time / hurtTime;

            CallFriend();
            yield return null;
        }

        if (time >= hurtTime)
        {
            curFirstState = FirstState.Dead; //시간 지나면 죽음
        }
        else if (isGotShot)
        {
            curFirstState = FirstState.Discover; //총에 맞았으면 발각, 게임오버
        }
        else
        {
            curFirstState = FirstState.Idle;
        }
    }
    
    #endregion
    
    //애니메이션 상태 바꾸기
    protected virtual void ChangeFirstAnim(FirstState changeFirstState)
    {
        for (int i = 0; i < stateValues.Length; i++)
        {
            if (changeFirstState != FirstState.Dead)
            {
                anim.SetBool($"is{(FirstState)i}", changeFirstState == (FirstState)i);
            }
        }
    }
    
    protected virtual void ChangeSecondAnim(SecondState changeSecondState)
    {
        for (int i = 0; i < stateValues.Length; i++)
        {
            if (curFirstState != FirstState.Dead)
            {
                anim.SetBool($"is{(SecondState)i}", changeSecondState == (SecondState)i);
            }
            else
            {
                anim.SetBool($"is{(SecondState)i}", false);
            }
        }
    }
    
    //Obstacle 호출 메서드, 깨지는 소리 범위에 들어갔을때
    public void HearSound(Transform t)
    {
        target = t;
        //React();
    }
    
    //Obstacle 호출 메서드, Obstacle에 맞았을때
    //Player 호출 메서드, 총에 맞았을때
    public void Hit(bool isGunShot, Transform trans)
    {
        isGotShot = isGunShot;
        //todo 헤드샷체크
        // if (hitCollider != null && hitCollider == headCollider)
        // {
        //     curState = State.Dead;
        // }
        // else
        // {
        //     curState = State.Hurt;
        // }
        
        //todo 피격위치에서 소리발생
    }
    
    //소리에 대한 반응
    // protected virtual void React()
    // {
    //     if(curFirstState is FirstState.Dead or FirstState.Discover) return;
    //     switch (curFirstState)
    //     {
    //         case FirstState.Dead: case FirstState.Discover:
    //             return;
    //         case FirstState.See: 
    //             curFirstState = FirstState.Scream;
    //             break;
    //         case FirstState.Careful:
    //             curFirstState = FirstState.Discover;
    //             break;
    //         default:
    //             //경계
    //             curFirstState = FirstState.Careful;
    //             break;
    //     }
    // }
    
    //시야 범위 관찰대상쪽으로 레이 쏘기 (관찰 상태가 아닐때)
    private void OnTriggerStay(Collider other)
    {
        // 임시 비활성화
        //if(curState is State.Scream or State.See) return; 
        //if (other.CompareTag("Evidence")) 
        //{
        //    Vector3 dirToTarget = (other.transform.position - transform.position).normalized;
        //    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        //    {
        //        float dst = Vector3.Distance(transform.position, other.transform.position);
        //        if (!Physics.Raycast(transform.position, dirToTarget, dst, Evidence))
        //        {
        //            Debug.DrawRay(transform.position, dirToTarget, Color.red);
        //            curState = State.Scream;
        //            target = other.transform;
        //        }
        //    }
        //  }
    }
}
