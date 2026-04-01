using UnityEngine;

public class SwitchBoard : Obstacle		//건물 배전반
{
	public StreetLamp[] lights; //가로등의 전구부분(Lamp)
	private bool isBroken = false;      //배전반 파괴여부
	public GameObject brokenEffect;
	public GameObject sparkEffect;

	protected override void UniqueInteraction() //고유한 작용
    {
		if (isBroken)
		{
			return;
		}
		sparkEffect.SetActive(true);

		base.UniqueInteraction();   //산산조각 나면 안되므로 RuntimeFracturedGeometry컴포넌트 내의 프리팹 교체 필요


		isBroken = true;
		if (brokenEffect != null)
		{
			brokenEffect.SetActive(true);
			Instantiate(brokenEffect, brokenEffect.transform.position, Quaternion.identity);
		}
		
		foreach (StreetLamp light in lights)
		{
			//light.SetActive(false);
			light.TurnOff();
		}
	}

	protected override void OnCollisionEnter(Collision collision) { }
}
