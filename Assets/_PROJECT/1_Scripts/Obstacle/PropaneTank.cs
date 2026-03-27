using UnityEngine;

public class PropaneTank : Obstacle
{
	public float explosionRadius = 5f;	//폭발 범위
	public float explosionForce = 2000f;	//폭발 세기

	protected override void UniqueInteraction()
	{
		//if (Explosives[i] == this) continue;
		base.UniqueInteraction();

		//폭발 반경 주위 모든 콜라이더 객체 찾기
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (Collider hit in colliders)
		{
			if(hit.gameObject == this.gameObject)
			{
				continue;
			}

			Character character = hit.GetComponent<Character>();	//각 객체에 Rigidbody가 있는지 찾기

			if (character != null)
			{
				//폭발 중심에서 바깥으로 날리기(폭발 세기, 폭발 중심점, 폭발 반경)
				character.Hit(false, hit);

				//TODO : 캐릭터의 사망이나 모션 등 작성이 필요함
			}

			//if (hit.CompareTag("Obstacle"))
			//{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if(hit.gameObject.GetComponent<Rigidbody>() == null)
				{
					rb = hit.gameObject.AddComponent<Rigidbody>();
				}
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
				Obstacle ob = hit.GetComponent<Obstacle>();
				if (ob != null)
				{
					ob.Hit(transform);
				}
			//}

		}

		/*
		 foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();	//각 객체에 Rigidbody가 있는지 찾기

			if (rb != null)
			{
				//폭발 중심에서 바깥으로 날리기(폭발 세기, 폭발 중심점, 폭발 반경)
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

				//TODO : 캐릭터의 사망이나 모션 등 작성이 필요함
			}
		}
		*/
		//폭발 후 비활성화or삭제 용도(산산조각 내는 경우 사용하지 않음)
		//gameObject.SetActive(false);
		//Destroy(gameObject);
	}
}
