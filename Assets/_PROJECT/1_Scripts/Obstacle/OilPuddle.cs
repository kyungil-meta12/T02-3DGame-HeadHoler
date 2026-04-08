using System.Collections;
using UnityEngine;

public class OilPuddle : Obstacle
{
	[Header("기름 확산 설정")]
	public float spreadTime = 10f;
	public float maxRadius = 3f;        //퍼지는 범위
	[Header("기름 불타서 없어지는 시간")]
	public float reduceTime = 10f;

	private float elapsed = 0f;
	private float elapsed2 = 0f;

	public GameObject fireEffect;       //vfx_Flames_01 1
	private bool isBurn = false;

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

		yield return new WaitUntil(() => fireEffect.activeSelf);
		//fireEffect가 켜지기 전까지 대기, fireEffect가 켜진 것을 감지하고 scale 0까지줄이기, scale 0 되면 Destroy
		while (elapsed < reduceTime)
		{
			elapsed2 += Time.deltaTime;

			float spreadOil = elapsed2 / reduceTime;

			transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, spreadOil);

			yield return null;
		}
		
		Destroy(gameObject);
	}

	protected override void UniqueInteraction()
	{
		if (fireEffect != null && !isBurn)
		{
			fireEffect.SetActive(true);
			isBurn = true;

			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb == null)
			{
				rb = gameObject.AddComponent<Rigidbody>();
			}

			rb.useGravity = false;
			rb.isKinematic = false;
			rb.constraints = RigidbodyConstraints.FreezeAll;

			StartCoroutine(ForceCollisionCheck());
		}
	}

	IEnumerator ForceCollisionCheck()
	{
		Collider col = GetComponent<Collider>();
		if (col != null)
		{
			col.enabled = false;
			yield return new WaitForFixedUpdate();
			col.enabled = true;
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		if (isBurn && collision.gameObject.CompareTag("Entity"))
		{
			//entity Hit()
			collision.gameObject.GetComponentInParent<Entity>().Hit(
				collision.collider,transform.position-collision.contacts[0].point, damage);
			var hitSound = collision.transform.GetComponentInParent<HitSound>();
			//entity 불태우기
			if (hitSound == null)
			{
				var hitEvidence = Instantiate(hitSoundPrefab, collision.transform.GetComponentInParent<Evidence>().transform);
				hitEvidence.GetComponent<HitSound>().isBurn = true;
			}
		}
	}
}
