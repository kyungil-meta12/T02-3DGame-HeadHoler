using UnityEngine;
using System.Collections;

public class SecurityCameraController : Obstacle
{
	[Header("카메라 움직임 설정")]
	public float waitTime = 3f;
	public float sweepAngle = 45f;
	public float rotateTime = 10f;

	private bool firstMove = true;

	[Header("시야 설정")]
	public float viewRadius = 20f;
	[Range(10f, 45f)]
	public float viewAngle = 15f;
	
	private float detectionTiltX = 15f;	//감지각도 15도 보정(카메라 실제 rotation값과 프리팹 카메라 방향차이 보정용)

	public LayerMask ragdollLayer;
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

		StartCoroutine(FindTarget(0.2f));
	}

	//카메라 움직임
	IEnumerator CameraMoveRoutine()
	{
		yield return new WaitForSeconds(waitTime);

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

			yield return new WaitForSeconds(waitTime);

			targetAngle = (targetAngle == sweepAngle) ? -sweepAngle : sweepAngle;	//양측 각도 끝으로 번갈아가면서 움직임
		}
	}

	//0.2초마다 시야내 타겟 찾기
	IEnumerator FindTarget(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	//타겟 찾기
	private void FindVisibleTargets()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, ragdollLayer);   //시야거리 내 시체 찾기(ragdoll레이어 설정 필요)

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
				}
			}
		}
	}
}
