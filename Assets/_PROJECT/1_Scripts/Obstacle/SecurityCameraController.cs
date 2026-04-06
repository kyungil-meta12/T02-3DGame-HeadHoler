using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;

public class SecurityCameraController : Obstacle
{
	[Header("알림을 전달할 Entity")]
	public Entity observerEntity;
	private BehaviorGraphAgent behaviorGraphAgent;
	
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
	
	private BlackboardVariable<GameObject> alertTarget;
	private BlackboardVariable<List<GameObject>> fracturedTargets;
	private HashSet<GameObject> targetSet;

	private void Start()
	{
		if (spotLight != null) //본체 시야각, 시야거리를 SpotLight에 적용
		{
			spotLight.range = viewRadius;
			spotLight.spotAngle = viewAngle;
		}

		if (observerEntity != null)
		{
			behaviorGraphAgent = observerEntity.GetComponentInParent<BehaviorGraphAgent>();
			behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out alertTarget);
			behaviorGraphAgent.GetVariable<List<GameObject>>("FracturedTargets", out fracturedTargets);
			targetSet = new HashSet<GameObject>(fracturedTargets.Value);
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
			if (observerEntity == null) yield break;
			if (!observerEntity.isDead)
			{
				FindVisibleTargets();
			}
		}
	}
	
	//타겟 찾기
	//배열 미리 선언해서 가비지 방지 
	private Collider[] targetsInViewRadius = new Collider[100];
	private void FindVisibleTargets()
	{
		int hitCount = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetsInViewRadius);
		
		for (int i = 0; i < hitCount; i++)
		{
			if (targetsInViewRadius[i].gameObject.CompareTag("Evidence") || targetsInViewRadius[i].gameObject.CompareTag("FracturedObject"))
			{
	            //시야각 검사
	            Vector3 dirToTarget = (targetsInViewRadius[i].transform.position - transform.position).normalized;
	            float dst = Vector3.Distance(transform.position, targetsInViewRadius[i].transform.position);
	            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
	            {
	                //가리는 것이 없는지 검사, obstacle이나 entity인지 검사
	                Ray ray = new Ray(transform.position, dirToTarget);
	                bool isHitEntity = Physics.Raycast(ray, dst, 1 << LayerMask.NameToLayer("Entity"));
	                bool isHitObstacle = Physics.Raycast(ray, dst, 1 << LayerMask.NameToLayer("Obstacle"));
	                if (isHitEntity)
	                {
	                    //부모가 evidence인지 검사
	                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
	                    Evidence evidence = targetsInViewRadius[i].gameObject.GetComponentInParent<Evidence>();
	                    if (evidence != null)
	                    {
	                        //AlertTarget이 동일한지 검사
	                        if (alertTarget.Value != evidence.gameObject)
	                        {
	                            alertTarget.Value = evidence.gameObject;
	                        }
	                    }
	                }
	                else if (isHitObstacle)
	                {
	                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
	                    
	                    //FracturedTargets 리스트에 있는지 검사
                        if (!targetSet.Contains(targetsInViewRadius[i].gameObject))
                        {
	                        targetSet.Add(targetsInViewRadius[i].gameObject);
                        }
	                }
	                else
	                {
	                    Debug.DrawRay(transform.position, dirToTarget, Color.red);
	                }
	            }
	        }
		}
		behaviorGraphAgent.SetVariableValue<List<GameObject>>("FracturedTargets", targetSet.ToList());
	}
	protected override void UniqueInteraction()
	{
		StopAllCoroutines();
		base.UniqueInteraction();
	}
	protected override void OnCollisionEnter(Collision collision) { }
}
