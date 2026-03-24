using UnityEngine;

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
