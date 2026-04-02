using UnityEngine;
using System.Collections;
using Unity.Behavior;

public class SecurityCameraController : Obstacle
{
	[Header("소속 팀(알림을 전달할 팀)")]
	public Team team;
	
	[Header("카메라 움직임 설정")]
	public static float waitTime = 3f;
	public float sweepAngle = 45f;
	public float rotateTime = 10f;

	private bool firstMove = true;

	[Header("시야 설정")]
	public float viewRadius = 20f;
	[Range(10f, 45f)]
	public float viewAngle = 15f;
	
	private float detectionTiltX = 15f;	//감지각도 15도 보정(카메라 실제 rotation값과 프리팹 카메라 방향차이 보정용)

	public LayerMask evidenceLayer;
	public LayerMask ObstacleLayer;
	public Light spotLight;

	private void Start()
	{
		if (spotLight != null)	//본체 시야각, 시야거리를 SpotLight에 적용
		{
			spotLight.range = viewRadius;
			spotLight.spotAngle = viewAngle;
		}

		StartCoroutine(CameraMoveRoutine());

		StartCoroutine(FindTarget());
	}

	//카메라 움직임
	private WaitForSeconds cameraMoveWait = new WaitForSeconds(waitTime);
	IEnumerator CameraMoveRoutine()
	{
		yield return cameraMoveWait;

		float targetAngle = sweepAngle;

		while (true)
		{
			Quaternion startRotation = transform.localRotation;
			Quaternion endRotation = Quaternion.Euler(0, targetAngle, 0);

			float time = 0f;
			float firstRotation = 1f;

			if (firstMove == true)	//초기 회전값 0(중앙) 상정
			{
				firstMove = false;
				firstRotation = 2f;
			}
			while (time < 1f)
			{
				time += Time.deltaTime / (rotateTime / firstRotation);
				transform.localRotation = Quaternion.Lerp(startRotation, endRotation, time);
				yield return null;
			}

			yield return cameraMoveWait;

			targetAngle = (targetAngle == sweepAngle) ? -sweepAngle : sweepAngle;	//양측 각도 끝으로 번갈아가면서 움직임
		}
	}

	//0.2초마다 시야내 타겟 찾기
	private WaitForSeconds findWait = new WaitForSeconds(0.2f);
	IEnumerator FindTarget()
	{
		while (true)
		{
			yield return findWait;
			FindVisibleTargets();
		}
	}

	//타겟 찾기
	private void FindVisibleTargets()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, evidenceLayer);   //시야거리 내 시체 찾기(ragdoll레이어 설정 필요)

		Vector3 tiltedForward = transform.rotation * Quaternion.Euler(detectionTiltX, 0, 0) * Vector3.forward;

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 targetDir = (target.position - transform.position).normalized;

			if (Vector3.Angle(tiltedForward, targetDir) < viewAngle / 2)	//시야각 제약
			{
				float targetDist = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, targetDir, targetDist, ObstacleLayer))	//카메라와 시체 사이에 장애물이 없는 경우(장애물 레이어 설정 필요)
				{
					Debug.Log("시체 발견");

					//Todo : NPC 호출 또는 경보나 경계상태

					foreach (var entity in Sg_GameManager.Inst.entities)
					{
						if(entity.myTeam != team) continue;
						//entity.GetComponent<BehaviorGraphAgent>().SetVariableValue("AlertTarget", )
					}
				}
			}
		}
	}
	//Todo : 카메라를 사격하면 떨어지도록 구현. 가급적 조각나지 않고 그냥 떨어지게 할 예정
	protected override void UniqueInteraction()
	{
		base.UniqueInteraction();
	}
	protected override void OnCollisionEnter(Collision collision) { }
}
