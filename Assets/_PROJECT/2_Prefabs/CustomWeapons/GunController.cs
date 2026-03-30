using System;
using UnityEngine;
using Unity.Behavior;

public class GunController : MonoBehaviour
{
    // 격발 간격
    public float fireInterval;

    // 총 장탄수
    public int totalAmmo;

    // 현재 장탄수
    public int currAmmo;

    // 기본 대미지
    public float damage;

    // 반동
    public float recoil;

    // 스코프 캔버스
    public ScopeImageMove scopeImage;

    // 방아쇠를 당긴 상태 = 마우스 좌클릭 상태
    private bool triggerPulled = false;
    // 격발 간격 타이머
    private float fireTimer = 0f;

    void Start()
    {
        currAmmo = totalAmmo; // 탄창을 채운 상태로 시작
    }

    void Update()
    {
        fireTimer -= Time.deltaTime; // fireTimer가 0f가 되어야 격발 가능
        if (fireTimer < 0f)
        {
            fireTimer = 0f;
            if (triggerPulled)
            {
                FireGun();
            }
        }
    }

    // 격발
    void FireGun()
    {
        if (currAmmo == 0) // 총알이 없으면 격발 불가능
        {
            return;
        }

        // 화면 중앙으로 레이캐스팅 후 가까운 거리부터 오름차순 정렬
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hitList = Physics.SphereCastAll(ray, 0.1f, 999f, LayerMask.GetMask("Entity")); // 레이캐스팅은 Entity 레이어를 가진 객체에 대해서만 처리
      //  Debug.DrawRay(ray.origin, ray.direction * 999f, Color.red, 1.0f);
        Array.Sort(hitList, (a, b) => a.distance.CompareTo(b.distance));

        var inputDirection = ray.direction;
        inputDirection.y = 0f;

        // 하나라도 충돌이 발견되면 즉시 for문을 종료한다. => 관통 현상 방지
        foreach (var hit in hitList)
        {
            var behavComp = hit.transform.gameObject.GetComponentInParent<BehaviorGraphAgent>();
            var obstacleComp = hit.transform.gameObject.GetComponentInParent<Obstacle>();

            if (behavComp) // 사람인 경우
            {
                print("people hit");
                var regController = hit.transform.gameObject.GetComponentInParent<RagdollController>();
                if (behavComp.GetVariable<float>("curHP", out var bb))
                {
                    var currentHP = bb.Value;

                    if (hit.collider == regController.headCollider)
                    {
                        currentHP = 0f;
                        regController.headCollider.attachedRigidbody.AddForce(inputDirection * 200f, ForceMode.Impulse); // 맞은 방향으로 힘 가함
                        print("headshot");
                    }
                    else
                    {
                        foreach (var c in regController.ragdollColliders)
                        {
                            if (hit.collider == c)
                            {
                                currentHP -= damage;
                                currentHP = Mathf.Clamp(currentHP, 0f, 999f);
                                if (currentHP <= 0f)
                                {
                                    c.attachedRigidbody.AddForce(inputDirection * 200f, ForceMode.Impulse); // 죽은 이후에는 맞은 방향으로 힘 가함
                                }
                                print("not headshot");
                                print($"current HP: { currentHP }");
                                break;
                            }
                        }
                    }
                    bb.Value = currentHP;
                }

                break;
            }

            if (obstacleComp) // 상호작용 장애물인 경우
            {
                print("interactive obstacle hit");
                break;
            }

            if (!behavComp && !obstacleComp)  // 상호 작용 불가능한 장애물인 경우 // 예: 건물 외벽, 지형 등...
            {
                print("static obstacle hit");
                break;
            }
        }

        fireTimer += fireInterval; // 격발 타이머에 격발 간격 시간 추가
        currAmmo--; // 총알 1개 소모
        Sg_MouseMan.Inst.AddRecoil(recoil); // 반동으로 인해 화면이 위로 튄다
        scopeImage.AddRecoil(recoil); // 스코프 이미지에 진동 효과 추가

        print($"[GunController] Fire | Ammo: {currAmmo} / {totalAmmo}"); // 테스트용 출력
    }

    // 방아쇠 당기기/놓기
    public void SetGunTrigger(bool flag)
    {
        triggerPulled = flag;
    }

    public bool IsFired()
    {
        return fireTimer <= 0f;
    
    }
    // 재장전
    public void ReloadGun()
    {
        if(triggerPulled)
        {
            return;
        }    

        currAmmo = totalAmmo; // 탄창 교체

        print($"[GunController] Reload | Ammo: {currAmmo} / {totalAmmo}"); // 테스트용 출력
    }
}
