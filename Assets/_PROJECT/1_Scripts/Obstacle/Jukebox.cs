using UnityEngine;

public class Jukebox : Obstacle
{
	[Header("주크박스 버튼HP")]
	public int buttonHP = 3; //버튼 명중시 주크박스 HP(버티는 횟수)
	private int buttonHitCount = 0; //버튼 명중 횟수
	[Header("유인범위")]
	public float attractionRadius = 10f;  //NPC 끌어당길 범위
	[Header("파괴 이펙트")]
	public GameObject destroyedSparkPrefab; //파괴 스파크
	public GameObject smokePrefab; //연기
	
	public void ButtonHit()	//자식 오브젝트(버튼) 명중 시 호출
	{
		buttonHitCount++;

		if(buttonHitCount >= buttonHP)
		{
			Hit(transform);
			return;
		}
		NotifyCharacter();
	}

	private void NotifyCharacter()
	{
		Collider[] listeners = Physics.OverlapSphere(transform.position, attractionRadius);

		foreach (var hit in listeners)
		{
			Character character = hit.GetComponent<Character>();

			if (character != null)
			{
				character.HearSound(transform);	//Character의 HearSound 활용
			}
		}
	}

	protected override void UniqueInteraction()
	{
		if (destroyedSparkPrefab != null && smokePrefab != null)
		{
			Instantiate(destroyedSparkPrefab, transform.position, Quaternion.identity);
			Instantiate(smokePrefab, transform.position, Quaternion.identity);
		}
		base.UniqueInteraction();
	}
}
