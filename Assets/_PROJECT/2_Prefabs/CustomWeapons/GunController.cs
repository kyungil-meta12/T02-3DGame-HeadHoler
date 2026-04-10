using System;
using UnityEngine;
using Unity.Behavior;
using UnityEngine.UI;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    //소리 발생 콜라이더 프리팹
    public GameObject hitSoundPrefab;

    // 총기 매쉬렌더러 // 줌 활성화 시 일시로 비활성화
    public MeshRenderer mr;
    public List<MeshRenderer> childMr = new();
    
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
    private Image centerImage;

    // 방아쇠를 당긴 상태 = 마우스 좌클릭 상태
    private bool triggerPulled = false;
    // 격발 간격 타이머
    private float fireTimer = 0f;

    void Awake() {
        currAmmo = totalAmmo; // 탄창을 채운 상태로 시작
        centerImage = scopeImage.transform.Find("ScopeImage").GetComponent<Image>();
        mr = GetComponent<MeshRenderer>();
        var mrs = mr.GetComponentsInChildren<MeshRenderer>();
        foreach(var m in mrs)
        {
            childMr.Add(m);
        }
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

    bool CheckMask(int collideMask, int targetMask)
    {
        return (collideMask & targetMask) != 0;
    }

    void CreateSoundCollider(Vector3 point)
    {
        if (hitSoundPrefab)
        {
            var hitSound = Instantiate(hitSoundPrefab, point, Quaternion.identity);
            hitSound.name = "hit(GunShot)";
            hitSound.GetComponent<HitSound>().isGunShot = true;
        }
    }

    // 격발
    void FireGun()
    {
        if (currAmmo == 0) // 총알이 없으면 격발 불가능
        {
            return;
        }
        
        var entityMask   = 1 << LayerMask.NameToLayer("Entity");
        var obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
        var groundMask   = 1 << LayerMask.NameToLayer("Ground");
        var wallMask     = 1 << LayerMask.NameToLayer("Wall");
        var wheelMask    = 1 << LayerMask.NameToLayer("Wheel");
        var carMask      = 1 << LayerMask.NameToLayer("Car");
        var findMask     = obstacleMask | entityMask | groundMask | wallMask | wheelMask | carMask;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        var rayDirection = ray.direction;
        rayDirection.y = 0f;
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 999f, Color.red, 10f);

        // 레이캐스팅은 findMask에 해당하는 레이어를 가진 객체에 대해서만 처리
        if (Physics.Raycast(ray, out var hit, 999f, findMask)) {
            var collideMask = 1 << hit.collider.gameObject.layer;

            // 사람인 경우
            if (CheckMask(collideMask, entityMask))
            {
                var entityComp  = hit.transform.gameObject.GetComponentInParent<Entity>();
                print("people hit");
                entityComp.Hit(hit.collider, rayDirection, damage);
                if (hitSoundPrefab)
                {
                    //사운드 콜라이더 생성, isGunShot = true
                    var hitSound = Instantiate(hitSoundPrefab, hit.transform.GetComponentInParent<Evidence>().transform);
                    hitSound.name = "hit(GunShot)";
                    hitSound.GetComponent<HitSound>().isGunShot = true;
                }
                Sg_HitIndicator.Inst.InputHit();
            }

            // 상호작용 장애물인 경우
            else if (CheckMask(collideMask, obstacleMask))
            {
                var obstacleComp = hit.transform.gameObject.GetComponentInParent<Obstacle>();
                if(obstacleComp) {
                    print("interactive obstacle hit");
                    obstacleComp.Hit(hit.point);
                    Sg_HitIndicator.Inst.InputHit();
                }

                CreateSoundCollider(hit.point);
                var rb = hit.transform.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(rayDirection * 10f, ForceMode.Impulse);
                }
            }

            // 차량 바퀴인 경우
            else if (CheckMask(collideMask, wheelMask))
            {
                var wheelComp    = hit.transform.gameObject.GetComponent<WheelController>();
                print("wheel hit");
                if (hitSoundPrefab)
                {
                    //사운드 콜라이더 생성, isGunShot = true
                    Instantiate(hitSoundPrefab, hit.point, Quaternion.identity);
                }
                wheelComp.DestroyWheel(rayDirection);
                wheelComp.DestroyWheel(ray.direction);
                Sg_HitIndicator.Inst.InputHit();
            }

            // 차량 경보 오브젝트인 경우
            else if (CheckMask(collideMask, carMask))
            {
                var carComp = hit.transform.gameObject.GetComponentInParent<CarObstacle>();
                if(carComp) {
                    print("car hit");
                    CreateSoundCollider(hit.point);
                    carComp.Hit();
                    Sg_HitIndicator.Inst.InputHit();
                }
            }

            // 그 외의 경우
            else
            {
                print("static obstacle hit");
                if (hitSoundPrefab)
                {
                    CreateSoundCollider(hit.point);
                }
            }
        }

        fireTimer += fireInterval; // 격발 타이머에 격발 간격 시간 추가
        currAmmo--; // 총알 1개 소모
        Sg_MouseMan.Inst.AddRecoil(recoil); // 반동으로 인해 화면이 위로 튄다
        scopeImage.AddRecoil(recoil); // 스코프 이미지에 진동 효과 추가

        Sg_SfxPlayer.Inst.PlayGunFire();

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

    // 매쉬렌더러 설정
    public void SetRenderState(bool flag)
    {
        mr.enabled = flag;
        foreach(var m in childMr)
        {
            m.enabled = flag;
        }
    }
}
