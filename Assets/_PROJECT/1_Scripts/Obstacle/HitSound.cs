
using System;
using System.Collections;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    [Header("사운드 발생 지속시간")]
    public float soundTimer = 3f;
    [Header("사운드 발생 범위")]
    public float maxSoundRadius = 5f;
    
    private SphereCollider hitSound;

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
        hitSound.radius = 0f;       //소리 감지영역 크기 초기화
        hitSound.enabled = true;
        float time = 0f;    //소리 퍼지는 시간 초기화

        while (time < soundTimer)
        {
            time += Time.deltaTime;
            float t = time / soundTimer;

            hitSound.radius = Mathf.Lerp(0f, maxSoundRadius, t);

            yield return null;
        }
    }
    
    private void OnTriggerEnter(Collider other)//소리범위에 시민이나 적이 닿았을때 Character의 메서드를 호출한다.
    {
        Character character = other.GetComponent<Character>();  //부딪친 오브젝트가 Character컴포넌트가 있는지 확인
        if(character != null)
        {
            character.HearSound(transform); //Character의 HearSound() 호출
        }
    }
}
