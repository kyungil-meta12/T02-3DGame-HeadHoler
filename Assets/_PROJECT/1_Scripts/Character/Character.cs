using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//todo 즉사가 아닐때
//todo 렉돌

public enum State
{
    //대기, 경계, 걷기, 뛰기, 동료부르기, 관찰, 저격위치 발각, 죽음
    Idle, Careful, Walk, Run, Scream, See, Discover, Dead
}

public class Character : MonoBehaviour
{
    [Header("순찰 위치")]
    public Vector3[] destinationsPos; // 자동 이동 지정위치
    [Header("시야각")]
    public float viewAngle = 90f;
    [Header("관찰대상 레이어")]
    public LayerMask Evidence;
    
    private Collider col; //감지용 콜라이더
    private Animator anim; //애니메이터
    private NavMeshAgent agent; //AI
    
    private State curState; //현재 상태
    private Array stateValues; //enum개수 체크용
    private Transform target;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    protected virtual void Start()
    {
        stateValues = Enum.GetValues(typeof(State));
        curState = State.Idle;

        StartCoroutine(MoveCoroutine());
    }

    //행동 루틴
    protected virtual IEnumerator MoveCoroutine()
    {
        while (true)
        {
            switch (curState)
            {
                case State.Idle:
                    yield return Idle();
                    break;
                case State.Careful:
                    yield return Careful();
                    break;
                case State.Walk:
                    yield return Walk();
                    break;
                case State.Run:
                    yield return Run();
                    break;
                case State.Scream:
                    yield return Scream();
                    break;
                case State.See:
                    yield return See();
                    break;
                case State.Discover:
                    yield return Discover();
                    break;
                case State.Dead:
                    yield return Dead();
                    break;
            }

            yield return null;
        }
    }

    //대기
    protected virtual IEnumerator Idle()
    {
        //일정시간 후 걷기
        ChangeAnim(State.Idle);
        yield return null;
    }

    //경계
    protected virtual IEnumerator Careful()
    {
        //일정시간 경계 후 대기
        ChangeAnim(State.Careful);
        yield return null;
    }
    
    //걷기
    protected virtual IEnumerator Walk()
    {
        //목표까지 걸어간 후 대기 or 걷는 중에 랜덤확률로 대기
        ChangeAnim(State.Walk);
        yield return null;
    }

    //뛰기
    protected virtual IEnumerator Run()
    {
        //목표까지 뛰어간 후 대기
        ChangeAnim(State.Run);
        yield return null;
    }

    //소리지르기
    protected virtual IEnumerator Scream()
    {
        //일정 시간 소리지른 후 관찰
        transform.forward = target.position - transform.position;
        ChangeAnim(State.Scream);
        yield return null;
    }
    
    //관찰
    protected virtual IEnumerator See()
    {
        //관찰대상 사라질때까지 관찰 후 대기 or 특정 상황 관찰 후 대기
        transform.forward = target.position - transform.position;
        ChangeAnim(State.See);
        yield return null;
    }

    //저격위치 발각
    protected virtual IEnumerator Discover()
    {
        //카메라 클로즈업, 발각 모션 후 게임 오버
        //transform.forward = 플레이어.position - transform.position;
        ChangeAnim(State.Discover);
        yield return null;
    }
    
    //죽음
    protected virtual IEnumerator Dead()
    {
        //행동 종료, 관찰대상 레이어로 바꾸기
        ChangeAnim(State.Dead);
        yield return null;
    }

    //애니메이션 상태 바꾸기
    protected virtual void ChangeAnim(State changeState)
    {
        for (int i = 0; i < stateValues.Length; i++)
        {
            anim.SetBool($"is{(State)i}", changeState == (State)i);
        }
    }
    
    //Obstacle 호출 메서드, 깨지는 소리 범위에 들어갔을때
    public void HearSound(Transform t)
    {
        target = t;
        React();
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
        if(curState is State.Scream or State.See) return;
        if (other.CompareTag("Evidence")) 
        {
            Vector3 dirToTarget = (other.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dst = Vector3.Distance(transform.position, other.transform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dst, Evidence))
                {
                    Debug.DrawRay(transform.position, dirToTarget, Color.red);
                    curState = State.Scream;
                    target = other.transform;
                }
            }
        }
    }
}
