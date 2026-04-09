using DinoFracture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
	Window,
	GasBarrel,
	OilBarrel,
	SwitchBoard,
	AirVent,
	Box
}

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
	
	//todo Entity의 Hit(RaycastHit hit, Vector3 direction, float dmg) 호출해서 데미지 입히기

	public ObstacleType obstacleType;

	public GameObject hitSoundPrefab;

	public ExplosionEffect explosionEffect; // 폭발 파티클 프리펩
	public float explosionEffectScale; // 폭발 파티클 크기
	public float explosionShakeEffectStrength; // 폭발 진동 효과 강ㄷ도
	
	public FractureGeometry[] explosives;   //파괴할 오브젝트

	public bool fragile; //부서질수있음

	public float damage = 50f;
	
	protected Rigidbody rb;

	[ContextMenu("테스트")]
	public void Test()
	{
		Hit(transform.position);
	}

	public void PlayHitSound()
	{
		switch(obstacleType)
		{
			case ObstacleType.GasBarrel:
				Sg_SfxPlayer.Inst.PlayMetalHit();
				break;
			
			case ObstacleType.OilBarrel:
				Sg_SfxPlayer.Inst.PlayMetalHit();
				break;

			case ObstacleType.SwitchBoard:
				Sg_SfxPlayer.Inst.PlayMetalHit();
				break;

			case ObstacleType.AirVent:
				Sg_SfxPlayer.Inst.PlayMetalHit();
				break;
			
			case ObstacleType.Box:
				Sg_SfxPlayer.Inst.PlayBoxHit();
				break;
		}
	}

	public void PlayDestroySound()
	{
		// 타입에 따라 다른 사운드를 재생한다
		switch(obstacleType)
		{
			case ObstacleType.Window:
				Sg_SfxPlayer.Inst.PlayWindowBreak();
				break;

			case ObstacleType.GasBarrel:
				Sg_SfxPlayer.Inst.PlayObstacleExplode();
				break;
			
			case ObstacleType.OilBarrel:
				break;

			case ObstacleType.SwitchBoard:
				break;

			case ObstacleType.AirVent:
				Sg_SfxPlayer.Inst.PlayMetalHit();
				break;

			case ObstacleType.Box:
				break;
		}
	}

	public virtual void Hit(Vector3 trans) //총알에 맞았을때
	{
		PlayHitSound();
		UniqueInteraction();
	}

	// fracture 시스템이 호출
	public void OnObstacleDestroy()
	{
		// 폭발 파티클 추가 // 프리펩 없으면 생략
		if (explosionEffect)
		{
			var exp = Instantiate(explosionEffect, transform.position, Quaternion.identity);
			exp.transform.localScale = new Vector3(explosionEffectScale, explosionEffectScale, explosionEffectScale);
			Sg_CameraController.Inst.AddShake(explosionShakeEffectStrength);
		}
		PlayDestroySound();
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

					explosives[i].Fracture();
				}
			}
		}
		
		
		//자신 분쇄
		
		if (fragile && TryGetComponent<RuntimeFracturedGeometry>(out RuntimeFracturedGeometry fracture))
		{
			if (gameObject.GetComponent<Rigidbody>() == null)
			{
				rb = gameObject.AddComponent<Rigidbody>();
				rb.useGravity = false;
			}

			if (fracture != null)
			{
				fracture.Fracture();
			}
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("FracturedObject"))
		{
			Hit(other.transform.position);
		}
	}
	
	public virtual void ScanComplete()
	{
		Destroy(gameObject);
	}
}