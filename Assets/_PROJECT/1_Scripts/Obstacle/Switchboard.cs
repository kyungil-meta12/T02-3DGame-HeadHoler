using UnityEngine;

public class SwitchBoard : Obstacle		//건물 배전반
{
	[SerializeField]
	private GameObject connectedBuilding;	//정전되는 건물
	private bool isBroken = false;		//배전반 파괴여부

    protected override void UniqueInteraction() //고유한 작용
    {
		if (isBroken)
		{
			return;
		}

		base.UniqueInteraction();

		isBroken = true;
		BlackOut();		//정전을 시킨다.
		Debug.Log($"{gameObject.name} 건물이 정전되었습니다");
    }

	private void BlackOut()
	{
		if (connectedBuilding != null)
		{
			Light[] lights = connectedBuilding.GetComponentsInChildren<Light>();
			foreach (Light light in lights)
			{
				light.enabled = false;
			}
		}
	}
}
