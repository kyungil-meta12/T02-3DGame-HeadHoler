
using System.Collections;
using UnityEngine;

public class CharacterCall : MonoBehaviour
{
    private Character character;
    private SphereCollider callSound;
    
    private Coroutine callSoundCoroutine;

    private void Awake()
    {
        callSound = GetComponent<SphereCollider>();
        character = GetComponent<Character>();
    }

    public void Call()
    {
        if (callSoundCoroutine == null)
        {
            callSoundCoroutine = StartCoroutine(SoundCoroutine());
        }
    }
    
    private IEnumerator SoundCoroutine() //소리범위를 늘려준다.
    {
        callSound.radius = 0f;
        callSound.enabled = true;
        float time = 0f;

        while (time < character.soundTimer)
        {
            time += Time.deltaTime;
            float t = time / character.soundTimer;

            callSound.radius = Mathf.Lerp(0f, character.maxSoundRadius, t);

            yield return null;
        }
    }
    
    private void OnTriggerEnter(Collider other)//소리범위에 시민이나 적이 닿았을때 Character의 메서드를 호출
    {
        Character otherCharacter = other.GetComponent<Character>();
        if(otherCharacter != null)
        {
            otherCharacter.HearSound(transform);
        }
    }
}
