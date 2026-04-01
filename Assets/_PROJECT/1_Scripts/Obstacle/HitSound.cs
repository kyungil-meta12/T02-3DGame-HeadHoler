
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    [Header("사운드 발생 지속시간")]
    public float soundTimer = 10f;
    [Header("사운드 발생 범위")]
    public float maxSoundRadius = 5f;
    
    private SphereCollider hitSound;
    public bool isGunShot = false;

    private void Awake()
    {
        hitSound = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        StartCoroutine(SoundCoroutine());
        Destroy(gameObject, soundTimer);
    }
    private IEnumerator SoundCoroutine() //소리범위를 늘려준다.
    {
        hitSound.radius = 0f;
        hitSound.enabled = true;
        float time = 0f; 

        while (time < soundTimer)
        {
            time += Time.deltaTime;
            float t = time / soundTimer;

            hitSound.radius = Mathf.Lerp(0f, maxSoundRadius, t);

            yield return null;
        }
        hitSound.enabled = false;
    }
    
    //소리범위에 닿았을때 Entity AlertTarget에 해당 지점 참조
    private void OnTriggerEnter(Collider other)
    {
        // Character character = other.GetComponent<Character>();  //부딪친 오브젝트가 Character컴포넌트가 있는지 확인
        // if(character != null)
        // {
        //     character.HearSound(transform); //Character의 HearSound() 호출
        // }

        if (other.CompareTag("Entity"))
        {
            other.gameObject.GetComponent<BehaviorGraphAgent>().SetVariableValue("AlertTarget", gameObject);
        }
    }
}
