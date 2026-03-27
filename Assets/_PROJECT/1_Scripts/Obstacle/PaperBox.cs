using UnityEngine;

public class PaperBox : Obstacle
{
	public GameObject paperBundlePrefab; //종이뭉치 프리팹
	public float spreadRotation = 3f; //종이가 회전하는 속도


	public override void Hit(Transform trans)
	{
		base.Hit(trans);
		//높은 곳에서 떨어질 때의 호출 구상해야 함
	}

	protected override void UniqueInteraction()
	{
		if(paperBundlePrefab != null)
		{
			//종이뭉치 생성
			GameObject bundle = Instantiate(paperBundlePrefab, transform.position, transform.rotation);

			Rigidbody[] papers = bundle.GetComponentsInChildren<Rigidbody>();
			
			foreach (Rigidbody rb in papers)
			{
				float randomUpForce = Random.Range(1.2f, 2.0f);
				
				//종이들이 위로 날리는 힘
				rb.AddForce(Vector3.up * randomUpForce, ForceMode.Impulse);
			
				//퍼지면서 회전
				rb.AddTorque(Random.insideUnitSphere * spreadRotation, ForceMode.Impulse);
			
				//약간 수평으로 퍼뜨림
				Vector3 slightScatter = new Vector3(Random.Range(-0.9f, 0.9f), 0f, Random.Range(-0.9f, 0.9f));
				rb.AddForce(slightScatter, ForceMode.Impulse);
			}
			
			base.UniqueInteraction();
			gameObject.SetActive(false); //종이박스 삭제
		}
	}
}
