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
}
