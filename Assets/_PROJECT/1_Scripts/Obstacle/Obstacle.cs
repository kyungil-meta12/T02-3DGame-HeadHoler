using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	/*
    public 필드 //인스펙터에서 관리해야 된다, 다른 스크립트가 호출해야 된다.
    
    private 필드 //자식이 몰라도 부모가 쓰면 된다.
    소리 감지영역 콜라이더
    
    메서드
    Hit() : 플레이어가 총을 레이캐스트로 쏘면 호출할 메서드
    OnColliderEnter(Collider other)
    콜라이더 크기 조절 코루틴

    알고리즘
    플레이어가 레이캐스트를 쏴서 맞았을때 Hit()호출
    소리 감지영역 콜라이더를 킨다.
    콜라이더 범위를 0부터 적절한 범위까지 적절한 시간 텀으로 늘려줄 코루틴을 호출한다.
    콜라이더 영역에 시민이나 적의 충돌을 감지한다.
    시민이나 적의 메서드를 호출한다. -> Character의 메서드를 호출한다.
    상속을 자주 쓰는 이유
    시민 - 농부, 목수, ... 종류를 여러가지 추가해도 Character만 상속하면 Obstacle과 상호작용가능.
    */

	[SerializeField]
	private SphereCollider hitSound;	//소리 감지영역 담당 Collider
	[SerializeField]
	private float maxSoundRadius = 10f;	//소리 감지영역의 최대반경
	[SerializeField]
	private float duration = 1f;		//소리 감지영역이 퍼지는 시간

	private HashSet<Character> heard = new HashSet<Character>();	//범위 내 소리를 들은 Character 체크(중복 방지)

    protected virtual void Hit() //총알에 맞았을때
    {
		UniqueInteraction();
		StartCoroutine(SoundCoRoutine());
	}

	protected virtual void OnTriggerEnter(Collider other)//소리범위에 시민이나 적이 닿았을때 Character의 메서드를 호출한다.
	{
		Character character = other.GetComponent<Character>();	//부딪친 오브젝트가 Character컴포넌트가 있는지 확인

		if (character != null && !heard.Contains(character))	//Character가 있고 아직 소리를 들은 사람에 체크되지 않았으면
		{
			heard.Add(character);	//감지된 목록에 추가

			character.HearSound(transform.position);	//Character의 HearSound() 호출
		}
	}

	protected virtual IEnumerator SoundCoRoutine() //소리범위를 늘려준다.
    {
		heard.Clear();		//들은 사람 체크 초기화

		hitSound.radius = 0f;		//소리 감지영역 크기 초기화
		hitSound.enabled = true;
		float time = 0f;	//소리 퍼지는 시간 초기화


		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;

			hitSound.radius = Mathf.Lerp(0f, maxSoundRadius, t);
			
			yield return null;
		}

		hitSound.enabled = false;
	}

	protected virtual void UniqueInteraction() //고유한 작용
    {
        //깨진다
    }
}
