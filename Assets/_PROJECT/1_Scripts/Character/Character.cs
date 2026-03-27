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
    [Header("소리지르기 범위 프리팹")]
    public GameObject screamPrefab; //동료 부르는 범위 콜라이더
    [Header("소리지르기 지속시간")]
    public float soundTimer = 3f;
    [Header("소리지르기 범위")]
    public float maxSoundRadius = 5f;
    [Header("시야범위 프리팹")]
    public GameObject viewPrefab; //감지용 콜라이더
    [Header("시야각")]
    public float viewAngle = 90f; //색적범위
    [Header("걷기 속도")] 
    public float walkSpeed;
    [Header("뛰기 속도")] 
    public float runSpeed;

    [Header(("각 행동별 지속시간"))] 
    public float idleTime = 1f;
    public float carefulTime = 1f;
    public float screamTime = 1f;
    public int hurtTime = 5;
    
    private Animator anim; //애니메이터
    private NavMeshAgent agent; //AI

    internal FirstState curFirstState; //현재 하체 행동상태
    internal SecondState curSecondState; //현재 상체 행동상태
    private Array stateValues; //enum개수 체크용
    internal Transform target; //관찰대상
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
        
        Instantiate(viewPrefab, transform.position, transform.rotation, transform);
        Instantiate(screamPrefab, transform.position, transform.rotation, transform);

        StartCoroutine(FirstMoveCoroutine());
        StartCoroutine(SecondMoveCoroutine());
    }

    #region 기본 코루틴
    protected virtual IEnumerator FirstMoveCoroutine()
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
                    //todo 카메라 클로즈업, 발각 모션 후 게임 오버
                    //transform.forward = 플레이어.position - transform.position;
                    break;
                case FirstState.Dead:
                    transform.tag = "Evidence";
                    StopAllCoroutines();
                    break;
            }

            yield return null;
        }
    }
    protected virtual IEnumerator SecondMoveCoroutine()
    {
        while (true)
        {
            switch (curSecondState)
            {
                case SecondState.None:
                    yield return null;
                    break;
                case SecondState.Careful:
                    ChangeSecondAnim(SecondState.Careful);
                    yield return Careful();
                    break;
                case SecondState.Hurt:
                    ChangeSecondAnim(SecondState.Hurt);
                    yield return Hurt();
                    break;
                case SecondState.Scream:
                    ChangeSecondAnim(SecondState.Scream);
                    yield return Scream();
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
                yield break;
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
        //목표까지 걸어간 후 대기
        if (target == null)
        {
            //타겟이 없으면 기본 순찰루트 반복
            foreach (var dest in destinationsPos)
            {
                agent.speed = walkSpeed;
                agent.SetDestination(dest);
                while (true)
                {
                    if (curFirstState != FirstState.Walk)
                    {
                        agent.isStopped = true;
                        yield break;
                    }

                    if (Vector3.Distance(transform.position, dest) <= 1f) break;
                    yield return null;
                }
            }
        }
        else
        {
            //타겟이 있으면 타겟으로 이동
            agent.speed = walkSpeed;
            agent.SetDestination(target.position);
            while (true)
            {
                if (curFirstState != FirstState.Walk)
                {
                    agent.isStopped = true;
                    yield break;
                }

                if (Vector3.Distance(transform.position, target.position) <= 1f) break;
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
        if (target == null)
        {
            //타겟이 없으면 기본 순찰루트 반복
            foreach (var dest in destinationsPos)
            {
                agent.speed = runSpeed;
                agent.SetDestination(dest);
                while (true)
                {
                    if (curFirstState != FirstState.Run)
                    {
                        agent.isStopped = true;
                        yield break;
                    }

                    if (Vector3.Distance(transform.position, dest) <= 1f) break;
                    yield return null;
                }
            }
        }
        else
        {
            //타겟이 있으면 타겟으로 이동
            agent.speed = walkSpeed;
            agent.SetDestination(target.position);
            while (true)
            {
                if (curFirstState != FirstState.Walk)
                {
                    agent.isStopped = true;
                    yield break;
                }

                if (Vector3.Distance(transform.position, target.position) <= 1f) break;
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
            if (curFirstState != FirstState.See)
            {
                yield break;
            }
            transform.forward = target.position - transform.position;
            yield return null;
        }

        Character targetCharacter = target.GetComponent<Character>();
        if (targetCharacter != null)
        {
            if (targetCharacter.isGotShot)
            {
                curFirstState = FirstState.Discover;
                yield break;
            }
            else
            {
                Destroy(targetCharacter.gameObject);
            }
        }
        target = null;
        curFirstState = FirstState.Idle;
        curSecondState = SecondState.None;
    }
    #endregion

    #region SecondState 행동별 코루틴
    //경계
    protected virtual IEnumerator Careful()
    {
        //일정시간 경계 후 대기
        float time = 0f;
        while (time < 1)
        {
            if (curSecondState != SecondState.Careful)
            {
                yield break;
            }

            time += Time.deltaTime;
            float t = time / carefulTime;
            yield return null;
        }

        curSecondState = SecondState.None;
    }

    //소리지르기
    protected virtual IEnumerator Scream()
    {
        //일정 시간 소리지른 후 관찰
        CallFriend(target.position);
        
        float time = 0f;
        while (time < 1)
        {
            if (curSecondState != SecondState.Scream)
            {
                yield break;
            }
            time += Time.deltaTime;
            float t = time / screamTime;
            yield return null;
        }

        curFirstState = FirstState.See;
        curSecondState = SecondState.Careful;
    }

    //동료 부르기
    protected virtual void CallFriend(Vector3 dest)
    {
        //todo 범위 내 동료의 이동타겟 바꾸기,강제 이동
    }

    //상처
    WaitForSeconds callWait = new WaitForSeconds(2f);
    protected virtual IEnumerator Hurt()
    {
        //계속 동료부르기, 동료가 오면 일정시간 후 치료됨, 동료가 안오면 일정시간 후 과다출혈 사망
        for (int i = 0; i < hurtTime; i++)
        {
            CallFriend(transform.position);
            yield return callWait;
        }
        curFirstState = FirstState.Dead; //시간 지나면 죽음
        
        // if (isGotShot)
        // {
        //     curFirstState = FirstState.Discover; //총에 맞았으면 발각, 게임오버
        // }
        // else
        // {
        //     curFirstState = FirstState.Idle;
        // }
    }
    
    #endregion

    #region 애니메이션 상태 바꾸기
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
    #endregion

    #region 외부 호출 메서드
    //todo Obstacle 호출 메서드, 깨지는 소리 범위에 들어갔을때
    public void HearSound(Transform t)
    {
        target = t;
        React();
    }
    
    //Obstacle 호출 메서드, Obstacle에 맞았을때
    // todo Player 호출 메서드, 총에 맞았을때
    public void Hit(bool isGunShot, Transform trans)
    {
        isGotShot = isGunShot;
        
    }
    
    //소리에 대한 반응, 시체 발견시 반응
    public virtual void React()
    {
        if(curFirstState is FirstState.Dead or FirstState.Discover) return;
        switch (curSecondState)
        {
            case SecondState.None:
                curSecondState = SecondState.Careful;
                break;
            case SecondState.Careful:
                curSecondState = SecondState.Scream;
                break;
            case SecondState.Scream:
                curSecondState = SecondState.Scream;
                break;
            case SecondState.Hurt:
                curSecondState = SecondState.Hurt;
                break;
        }
    }
    #endregion
}
