using System.Collections;
using UnityEngine;

public class OilPuddle : Obstacle
{
	[Header("기름 확산 설정")]
	public float spreadTime = 10f;
	public float maxRadius = 3f;        //퍼지는 범위

	private float elapsed = 0f;

	public GameObject fireEffect;       //vfx_Flames_01 1

	private void Start()
	{
		StartCoroutine(SpreadPuddle());
	}

	IEnumerator SpreadPuddle()
	{
		Vector3 targetScale = new Vector3(maxRadius, 0.001f, maxRadius);
		Vector3 StartScale = new Vector3(0.001f, 0.001f, 0.001f);

		while (elapsed < spreadTime)
		{
			elapsed += Time.deltaTime;

			float spreadOil = elapsed / spreadTime;

			transform.localScale = Vector3.Lerp(StartScale, targetScale, spreadOil);

			yield return null;
		}

		transform.localScale = targetScale;
	}

	protected override void UniqueInteraction()
	{
		if (fireEffect != null)
		{
			fireEffect.SetActive(true);
		}
	}
}
