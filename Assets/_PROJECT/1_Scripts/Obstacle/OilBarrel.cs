using System.Collections;
using UnityEngine;

public class OilBarrel : Obstacle
{
	public GameObject oilStream;	//기름 줄기
	public GameObject oilPuddle;    //기름 웅덩이

	[Header("지면 레이어 설정")]
	public LayerMask groundLayer;   //**지면의 레이어를 설정할 것

	[Header("기름 웅덩이 설정")]
	public float puddleMaxRadius = 3f;
	public float puddleSpreadTime = 5f;
	public float puddleReduceTime = 10f;


	private bool isLeaking = false;

	//테스트용 함수
/*
	private void OnMouseDown()
	{
		if (isLeaking)  //드럼통을 이미 맞췄으면
		{
			return;
		}
		Vector3 hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit click;

		if (Physics.Raycast(ray, out click))
		{
			Debug.Log($"마우스클릭 감지 : {click.point}");
			hit = click.point;
			Hit(hit);
		}
	}*/

	// 총(또는 총탄)에서 위 HitPos 매개변수를 건네주기 위한 코드 예시
	// 임의 변수 : fpsCam
	/*
	RaycastHit hit;
	if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform, out hit, range))
	{
		//드럼통 명중 확인
		OilBarrel barrel = hit.transform.GetComponent<OilBarrel>();
		if (barrel != null)
		{
			barrel.OnShot(hit);	//맞은 정보 넘기기
		}

		//기름 웅덩이 명중 확인
		OilPuddle puddle = hit.transform.GetComponent<OilPuddle>();
		if (puddle != null)
		{
			puddle.UniqueInteraction();	//불 붙이기
		}
	}
	*/

	public override void Hit(Vector3 hit)	//명중 좌표
	{
		if (isLeaking)  //드럼통을 이미 맞췄으면
		{
			return;
		}

		isLeaking = true;

		if (oilStream != null)
		{
			//기름 줄기 회전값(새는 방향)
			Vector3 leakingDir = (hit - transform.position).normalized;
			leakingDir.y = 0;
			Quaternion leakingRotation = Quaternion.LookRotation(leakingDir);

			Quaternion lastRotation = leakingRotation * oilStream.transform.rotation;	//기름줄기 프리팹 회전값 적용

			GameObject stream = Instantiate(oilStream, hit, lastRotation);  //총알맞은 좌표에 기름 줄기 생성
			stream.transform.SetParent(this.transform); //드럼통을 부모로 기름 줄기 생성

			//웅덩이 생성좌표
			Transform dropPointMid = stream.transform.Find("DropPointMid");	//기름 줄기 중간좌표
			Transform dropPoint = stream.transform.Find("DropPoint");		//기름 줄기 끝좌표

			if (dropPoint != null && dropPointMid != null)
			{
				Vector3 puddlePos = Vector3.zero;

				//기름 줄기가 바닥에 떨어지는 좌표 계산(각 거리 계산)
				Vector3 streamTraceMid = (dropPointMid.position - hit).normalized; //명중좌표~중간좌표 방향 체크
				float distToMid = Vector3.Distance(hit, dropPointMid.position);	//명중좌표~중간좌표 거리

				Vector3 streamTraceEnd = (dropPoint.position - dropPointMid.position).normalized; //중간좌표~끝좌표 방향 체크
				float distToEnd = Vector3.Distance(dropPointMid.position, dropPoint.position);    //중간좌표~끝좌표 거리


				if (Physics.Raycast(hit, streamTraceMid, out RaycastHit groundHitMid, distToMid, groundLayer))	//중간좌표 방향 사이에 땅표면이 있으면
				{
					puddlePos = groundHitMid.point + (Vector3.up * 0.001f);
				}
				else if (Physics.Raycast(dropPointMid.position, streamTraceEnd, out RaycastHit groundHit, distToEnd, groundLayer)) //중간좌표~끝좌표 방향 사이에 땅표면이 있으면
				{
					puddlePos = groundHit.point + (Vector3.up * 0.001f);
				}
				else if (Physics.Raycast(dropPoint.position, Vector3.down, out RaycastHit groundHitFar, 100f, groundLayer)) //끝좌표까지 닿는 땅표면이 기름 줄기보다 멀어서 닿지 않으면
				{
					puddlePos = groundHitFar.point + (Vector3.up * 0.001f);	//끝좌표의 아래에 생성
				}
				else //예외 상황(땅 표면이 너무 멀거나 안잡히는 경우)
				{
					puddlePos = new Vector3(hit.x, transform.position.y + 0.001f, hit.z);	//드럼통 바닥에 생성
				}
				
				GameObject puddleObj = Instantiate(oilPuddle, puddlePos, Quaternion.identity);

				OilPuddle puddleComp = puddleObj.GetComponent<OilPuddle>();

				if (puddleComp != null)
				{
					puddleComp.maxRadius = this.puddleMaxRadius;
					puddleComp.spreadTime = this.puddleSpreadTime;
					puddleComp.reduceTime = this.puddleReduceTime;
				}

				//웅덩이 생성 후 기름 줄기 비활성화
				OilPuddle puddle = puddleObj.GetComponent<OilPuddle>();
				if (puddle != null)
				{
					float spreadTime = puddle.spreadTime + 0.3f;

					StartCoroutine(StopStream(stream, spreadTime));
				}
			}
		}
	}
	
	protected override void UniqueInteraction(){}
	protected override void OnCollisionEnter(Collision collision){}
	
	//기름 줄기 멈춤
	IEnumerator StopStream(GameObject stream, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (stream != null)
		{
			stream.SetActive(false);
		}
	}
}
