using UnityEngine;

public class StreetLamp : Obstacle
{
	public Material lightOn;
	public Material lightOff;
	public Light lampLight;

	private bool isBroken = false;      //전구 파괴여부
	public GameObject brokenEffect;

	private MeshRenderer lightMesh;

	private void Awake()
	{
		lightMesh = GetComponent<MeshRenderer>();
	}

	[ContextMenu("Turn Off")]
	public void TurnOff()
	{
		lightMesh.material = lightOff;

		if (lampLight != null)
		{
			lampLight.enabled = false;

			if (brokenEffect != null && isBroken == true) //배전반 파괴나 전원이 꺼질경우
			{
				brokenEffect.SetActive(false);  //스파크 비활성화
			}
		}
	}

	[ContextMenu("Turn On")]
	public void TurnOn()
	{
		lightMesh.material = lightOn;

		if (lampLight != null)
		{
			lampLight.enabled = true;

			if (brokenEffect != null && isBroken == true) //다시 복구되거나 켜질 경우
			{
				brokenEffect.SetActive(true);   //스파크 활성화
			}
		}
	}

	protected override void UniqueInteraction()
	{
		if (isBroken)	//이미 전구가 파괴된 경우
		{
			return;
		}

		base.UniqueInteraction();

		isBroken = true;
		if (brokenEffect != null && lampLight.enabled == true)	//전구 첫 파괴 시
		{
			brokenEffect.SetActive(true);
			//Instantiate(brokenEffect, brokenEffect.transform.position, Quaternion.identity);
		}
	}
}
