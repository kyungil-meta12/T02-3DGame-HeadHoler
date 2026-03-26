using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//todo 즉사가 아닐때

public enum State
{
    //대기, 경계, 걷기, 뛰기, 동료부르기, 관찰, 저격위치 발각, 상처, 죽음
    Idle, Careful, Walk, Run, Scream, See, Discover, Hurt, Dead
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
    [Header("상처 지속 시간")] 
    public float deathLimitTime = 10f; //과다출혈 사망시간
    
    private Animator anim; //애니메이터
    private NavMeshAgent agent; //AI
    
    private State curState; //현재 상태
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
        stateValues = Enum.GetValues(typeof(State));
        curState = State.Idle;

        // 레이캐스팅 테스트를 위해 임시 비활성화
        //StartCoroutine(MoveCoroutine());
    }

    #region 기본 코루틴
    protected virtual IEnumerator MoveCoroutine()
    {
        while (true)
        {
            switch (curState)
            {
                case State.Idle:
                    ChangeAnim(State.Idle);
                    yield return Idle();
                    break;
                case State.Careful:
                    ChangeAnim(State.Careful);
                    yield return Careful();
                    break;
                case State.Walk:
                    ChangeAnim(State.Walk);
                    yield return Walk();
                    break;
                case State.Run:
                    ChangeAnim(State.Run);
                    yield return Run();
                    break;
                case State.Scream:
                    ChangeAnim(State.Scream);
                    yield return Scream();
                    break;
                case State.See:
                    ChangeAnim(State.See);
                    yield return See();
                    break;
                case State.Discover:
                    ChangeAnim(State.Discover);
                    Discover();
                    break;
                case State.Hurt:
                    ChangeAnim(State.Hurt);
                    yield return Hurt();
                    break;
                case State.Dead:
                    ChangeAnim(State.Dead);
                    Dead();
                    break;
            }

            yield return null;
        }
    }
    #endregion

    #region 행동별 코루틴
    //대기
    protected virtual IEnumerator Idle()
    {
        //todo 일정시간 후 걷기
        float time = 0f;
        while (time < 1)
        {
            if (curState == State.Idle) break;
            time += Time.deltaTime;
            float t = time / 1;
            yield return null;
        }
    }

    //경계
    protected virtual IEnumerator Careful()
    {
        //todo 일정시간 경계 후 대기
        float time = 0f;
        while (time < 1)
        {
            if (curState == State.Careful) break;
            time += Time.deltaTime;
            float t = time / 1;
            yield return null;
        }
        curState = State.Idle;
    }
    
    //걷기
    protected virtual IEnumerator Walk()
    {
        //todo 목표까지 걸어간 후 대기 or 걷는 중에 랜덤확률로 대기
        while (true)
        {
            if (curState == State.Walk) break;

            foreach (var dest in destinationsPos)
            {
                agent.SetDestination(dest);
                while (Vector3.Distance(transform.position, dest) <= 1f)
                {
                    if (curState == State.Walk) break;
                    yield return null;
                }
            }
            yield return null;
        }

        curState = State.Idle;
    }

    //뛰기
    protected virtual IEnumerator Run()
    {
        //todo 목표까지 뛰어간 후 대기
        float time = 0f;
        while (time < 1)
        {
            if (curState == State.Run) break;
            time += Time.deltaTime;
            float t = time / 1;
            yield return null;
        }
        curState = State.Idle;
    }

    //소리지르기
    protected virtual IEnumerator Scream()
    {
        //일정 시간 소리지른 후 관찰
        if (target != null)
        {
            transform.forward = target.position - transform.position;
        }
        CallFriend();
        
        float time = 0f;
        while (time < 1)
        {
            if (curState == State.Scream) break;
            time += Time.deltaTime;
            float t = time / 1;
            yield return null;
        }

        curState = State.See;
    }

    //동료 부르기
    protected virtual void CallFriend()
    {
        //todo 범위 내 동료의 이동타겟 바꾸기,강제 이동
    }
    
    //관찰
    protected virtual IEnumerator See()
    {
        //관찰대상 사라질때까지 관찰 후 대기 or 특정 상황 관찰 후 대기
        while (target != null)
        {
            if(curState == State.See) break;
            transform.forward = target.position - transform.position;
            yield return null;
        }
        curState = State.Idle;
    }

    //상처
    protected virtual IEnumerator Hurt()
    {
        //자리에서 계속 소리지르기, 동료가 오면 일정시간 후 치료됨, 동료가 안오면 일정시간 후 과다출혈 사망
        float time = 0f;
        while (time < deathLimitTime)
        {
            time += Time.deltaTime;
            float t = time / deathLimitTime;

            CallFriend();
            yield return null;
        }

        if (time >= deathLimitTime)
        {
            curState = State.Dead; //시간 지나면 죽음
        }
        else if (isGotShot)
        {
            curState = State.Discover; //총에 맞았으면 발각, 게임오버
        }
        else
        {
            curState = State.Idle;
        }
    }
    
    //저격위치 발각
    protected virtual void Discover()
    {
        //todo 카메라 클로즈업, 발각 모션 후 게임 오버
        //transform.forward = 플레이어.position - transform.position;
    }
    
    //죽음
    protected virtual void Dead()
    {
        //행동 종료, 관찰대상 레이어로 바꾸기
        StopAllCoroutines();
        transform.gameObject.layer = Evidence;
    }
    #endregion
    
    //애니메이션 상태 바꾸기
    protected virtual void ChangeAnim(State changeState)
    {
        for (int i = 0; i < stateValues.Length; i++)
        {
            if (changeState != State.Dead)
            {
                anim.SetBool($"is{(State)i}", changeState == (State)i);
            }
        }
    }
    
    //Obstacle 호출 메서드, 깨지는 소리 범위에 들어갔을때
    public void HearSound(Transform t)
    {
        target = t;
        React();
    }
    
    //Obstacle 호출 메서드, Obstacle에 맞았을때
    //Player 호출 메서드, 총에 맞았을때
    public void Hit(bool isGunShot, Collider hitCollider)
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
    protected virtual void React()
    {
        switch (curState)
        {
            case State.Scream: case State.See: case State.Careful:
                curState = State.Scream;
                break;
            default:
                //경계
                curState = State.Careful;
                break;
        }
    }
    
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
