using UnityEngine;

public class Sg_SfxPlayer : MonoBehaviour
{
    public static Sg_SfxPlayer Inst;

    private AudioSource aSource;

    // 여기에 사운드를 추가하고 메서드를 추가하여 사운드 재생
    public AudioClip gunFire;
    public AudioClip magazineIn;
    public AudioClip magazineOut;
    public AudioClip reloadEnd;
    public AudioClip footstep;

    public AudioClip obstacleExplode;
    public AudioClip carExplode;
    public AudioClip windowBreak;
    public AudioClip wheelBreak;
    public AudioClip metalHit;
    public AudioClip boxHit;

    public AudioClip[] maleScream;
    public AudioClip[] femaleScream;

    public AudioClip headShot;
    public AudioClip scopeDial;

    public AudioClip carHorn;

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(this);

        aSource = GetComponent<AudioSource>();
    }

    public void PlayGunFire()
    {
        aSource.PlayOneShot(gunFire);
    }

    public void PlayMagazinOut()
    {
        aSource.PlayOneShot(magazineOut);
    }

    public void PlayMagazineIn()
    {
        aSource.PlayOneShot(magazineIn);
    }

    public void PlayReloadEnd()
    {
        aSource.PlayOneShot(reloadEnd);
    }

    public void PlayFootstepSound()
    {
        aSource.PlayOneShot(footstep);
    }

    public void PlayObstacleExplode()
    {
        aSource.PlayOneShot(obstacleExplode);
    }

    public void PlayCarExplode()
    {
        aSource.PlayOneShot(carExplode);
    }

    public void PlayWindowBreak()
    {
        aSource.PlayOneShot(windowBreak);
    }

    public void PlayWheelBreak()
    {
        aSource.PlayOneShot(wheelBreak);
    }

    public void PlayMetalHit()
    {
        aSource.PlayOneShot(metalHit);
    }

    public void PlayBoxHit()
    {
        aSource.PlayOneShot(boxHit);
    }

    public void PlayCarHorn()
    {
        aSource.PlayOneShot(carHorn);
    }

    public void PlayMaleScream()
    {
        int randNum = Random.Range(0, maleScream.Length - 1);
        aSource.PlayOneShot(maleScream[randNum]);
    }

    public void PlayFemaleScream()
    {
        int randNum = Random.Range(0, femaleScream.Length - 1);
        aSource.PlayOneShot(femaleScream[randNum]);
    }

    public void PlayHeadShot()
    {
        aSource.PlayOneShot(headShot);
    }

    public void PlayScopeDial()
    {
        aSource.PlayOneShot(scopeDial);
    }
}
