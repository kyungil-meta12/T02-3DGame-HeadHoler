using DinoFracture;
using System;
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

	public GameObject target;//부서질 오브젝트

	public SphereCollider hitSound; //소리 감지영역 담당 Collider
	public float maxSoundRadius = 10f;  //소리 감지영역의 최대반경
	public float duration = 1f;     //소리 감지영역이 퍼지는 시간

	public FractureGeometry[] Explosives;   //원격폭발(분쇄)할 오브젝트

	private void Awake()
	{
		hitSound = GetComponent<SphereCollider>();
	}

	[ContextMenu("테스트")]
	protected virtual void Hit() //총알에 맞았을때
	{
		UniqueInteraction();
		StartCoroutine(SoundCoroutine());
	}


	protected virtual void OnTriggerEnter(Collider other)//소리범위에 시민이나 적이 닿았을때 Character의 메서드를 호출한다.
	{
		Character character = other.GetComponent<Character>();  //부딪친 오브젝트가 Character컴포넌트가 있는지 확인

		character.HearSound(transform); //Character의 HearSound() 호출
	}

	protected virtual IEnumerator SoundCoroutine() //소리범위를 늘려준다.
	{
		hitSound.radius = 0f;       //소리 감지영역 크기 초기화
		hitSound.enabled = true;
		float time = 0f;    //소리 퍼지는 시간 초기화

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
		//dinoFracture 활성화
		//자신 산산조각, RuntimeFracturedGeometry컴포넌트 필요
		if (TryGetComponent<RuntimeFracturedGeometry>(out RuntimeFracturedGeometry fracture))
		{
			fracture.Fracture();
		}

		//원격폭발. 이 오브젝트(스위치)에 ExplodeOnFracture 컴포넌트 필요
		var exploder = GetComponent<ExplodeOnFracture>();

		for (int i = 0; i < Explosives.Length; i++)
		{
			if (Explosives[i] != null && Explosives[i].gameObject.activeSelf)
			{
				if (exploder != null)
				{
					// 폭발(사방으로 파편 비산) 실행	(외부 오브젝트에 RigidBody, PreFracturedGeometry컴포넌트 필요)
					Explosives[i].Fracture().SetCallbackObject(this);
				}
				else //스위치 오브젝트에 ExplodeOnFracture 컴포넌트 없으면
				{
					// 폭발없이 분쇄만 실행
					Explosives[i].Fracture();
				}
			}
		}
	}
}