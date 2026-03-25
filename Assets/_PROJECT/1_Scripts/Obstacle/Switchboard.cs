using UnityEngine;

public class SwitchBoard : Obstacle		//건물 배전반
{
	public GameObject[] lights; //가로등의 전구부분
	private bool isBroken = false;		//배전반 파괴여부

    protected override void UniqueInteraction() //고유한 작용
    {
		if (isBroken)
		{
			return;
		}

		base.UniqueInteraction();

		isBroken = true;
		
		foreach (GameObject light in lights)
		{
			light.SetActive(false);
		}
    }
}
