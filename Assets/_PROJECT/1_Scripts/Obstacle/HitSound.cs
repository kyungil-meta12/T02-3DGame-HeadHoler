
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    [Header("사운드 발생 지속시간")]
    public static float soundTimer = 1f;
    [Header("사운드 발생 범위")]
    public float maxSoundRadius = 20f;
    
    private SphereCollider hitSound;
    public bool isGunShot = false;

    private void Awake()
    {
        hitSound = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        StartCoroutine(SoundCoroutine());
        //Destroy(gameObject, soundTimer);
    }
    
    private WaitForSeconds wait = new WaitForSeconds(soundTimer);
    private IEnumerator SoundCoroutine() //소리범위를 늘려준다.
    {
        hitSound.radius = maxSoundRadius;
        hitSound.enabled = true;
        yield return wait;
        hitSound.enabled = false;
    }
    
    //소리범위에 닿았을때 Entity AlertTarget에 해당 지점 참조
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Entity"))
        {
            other.gameObject.GetComponentInParent<BehaviorGraphAgent>().GetVariable<GameObject>("AlertTarget", out var alertTarget);
            if (alertTarget.Value != gameObject)
            {
                alertTarget.Value = gameObject;
            }
        }
    }

    public void ScanComplete()
    {
        Destroy(gameObject);
    }
}
