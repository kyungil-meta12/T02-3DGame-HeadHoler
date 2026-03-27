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

	[Header("소리 발생용 프리팹")]
	public GameObject hitSoundPrefab; //소리 발생 콜라이더
	
	public FractureGeometry[] explosives;   //파괴할 오브젝트

	public bool fragile; //부서질수있음

	[ContextMenu("테스트")]
	public void Test()
	{
		Hit(transform);
	}
	public virtual void Hit(Transform trans) //총알에 맞았을때
	{
		Instantiate(hitSoundPrefab, trans.position, Quaternion.identity);
		UniqueInteraction();
	}
	
	protected virtual void UniqueInteraction() //고유한 작용
	{
		//dinoFracture 활성화
		
		//다른 오브젝트 분쇄
		if (explosives != null)
		{
			for (int i = 0; i < explosives.Length; i++)
			{
				if (explosives[i] != null)
				{
					if (explosives[i].GetComponent<Rigidbody>() == null)
					{
						Rigidbody otherRb = explosives[i].gameObject.AddComponent<Rigidbody>();
						otherRb.useGravity = false;
					}

					explosives[i].Fracture().SetCallbackObject(explosives[i]);
				}
			}
		}
		
		
		//자신 분쇄
		Rigidbody rb;
		if (fragile && TryGetComponent<RuntimeFracturedGeometry>(out RuntimeFracturedGeometry fracture))
		{
			if (gameObject.GetComponent<Rigidbody>() == null)
			{
				rb = gameObject.AddComponent<Rigidbody>();
				rb.useGravity = false;
			}

			if (fracture != null)
			{
				fracture.Fracture().SetCallbackObject(this);
			}
		}
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("FracturedObject"))
		{
			Hit(collision.transform);
		}
	}
}