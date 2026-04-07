
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
    
    public bool isGunShot = false;
    public BlackboardVariable<bool> isHurt;
    public bool isBurn = false;
    public GameObject fireEffect;

    private SphereCollider hitSound;
    private Entity hitEntity;
    private BehaviorGraphAgent behavior;

    private void Awake()
    {
        hitSound = GetComponent<SphereCollider>();
        hitEntity = GetComponentInParent<Entity>();
        behavior = GetComponentInParent<BehaviorGraphAgent>();
    }

    private void Start()
    {
        StartCoroutine(SoundCoroutine());
        //Destroy(gameObject, soundTimer);
        if (hitEntity != null && behavior != null)
        {
            if (behavior.GetVariable<bool>("isHurt", out isHurt))
            {
                if (isHurt)
                {
                    StartCoroutine(HurtCoroutine());
                    if (isBurn)
                    {
                        StartCoroutine(BurnCoroutine());
                    }
                }
            }
        }
    }

    private WaitForSeconds HurtWait =  new WaitForSeconds(3f);
    private IEnumerator HurtCoroutine()
    {
        
        while (true)
        {
            yield return HurtWait;
            
            if (isHurt)
            {
                hitEntity.currentHP -= 1;
            }
            else
            {
                isBurn = false;
                yield break;
            }
            
        }
        
        yield return null;
    }

    private WaitForSeconds BurnWait =  new WaitForSeconds(1f);
    private IEnumerator BurnCoroutine()
    {
        while (true)
        {
            if (isBurn)
            {
                hitEntity.currentHP -= 1;
                fireEffect.SetActive(true);
            }
            else
            {
                fireEffect.SetActive(false);
                yield break;
            }
            yield return BurnWait;
        }
        
        yield return null;
    }
    
    private WaitForSeconds SoundWait = new WaitForSeconds(soundTimer);
    private IEnumerator SoundCoroutine() //소리범위를 늘려준다.
    {
        hitSound.radius = maxSoundRadius;
        hitSound.enabled = true;
        yield return SoundWait;
        hitSound.enabled = false;
    }
    
    //소리범위에 닿았을때 Entity AlertTarget에 해당 지점 참조
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Entity"))
        {
            BehaviorGraphAgent behaviorGraphAgent = other.gameObject.GetComponentInParent<BehaviorGraphAgent>();
            behaviorGraphAgent.enabled = true;
            behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
            
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
