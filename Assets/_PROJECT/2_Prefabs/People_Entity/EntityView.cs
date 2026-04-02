
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EntityView : MonoBehaviour
{
    [Header("시야각")] 
    public float viewAngle = 90f;

    private Collider col;
    private Entity myEntity;
    private BehaviorGraphAgent behaviorGraphAgent;

    private void Awake()
    {
        col = GetComponent<Collider>();
        myEntity = GetComponentInParent<Entity>();
        behaviorGraphAgent = GetComponentInParent<BehaviorGraphAgent>();
    }

    private void OnTriggerStay(Collider other)
    {
        //시체, 증거물 태그 검사
        if (other.gameObject.CompareTag("Evidence"))
        {
            //시야각 검사
            Vector3 dirToTarget = (other.transform.position - transform.position).normalized;
            float dst = Vector3.Distance(transform.position, other.transform.position);
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //가리는 것이 없는지 검사, obstacle이나 entity인지 검사
                Ray ray = new Ray(transform.position, dirToTarget);
                bool isHitEntity = Physics.Raycast(ray, dst, 1 << LayerMask.NameToLayer("Entity"));
                bool isHitObstacle = Physics.Raycast(ray, dst, 1 << LayerMask.NameToLayer("Obstacle"));
                if (isHitEntity)
                {
                    //부모가 entity인지 검사
                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
                    Entity entity = other.gameObject.GetComponentInParent<Entity>();
                    if (entity != null)
                    {
                        //AlertTarget이 동일한지 검사
                        behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
                        if (alertTarget.Value != entity.gameObject)
                        {
                            alertTarget.Value = entity.gameObject;
                        }
                    }
                }
                else if (isHitObstacle)
                {
                    //부모가 obstacle인지 검사
                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
                    Obstacle obstacle = other.gameObject.GetComponentInParent<Obstacle>();
                    if (obstacle != null)
                    {
                        //AlertTarget이 동일한지 검사
                        behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
                        if (alertTarget.Value != obstacle.gameObject)
                        {
                            alertTarget.Value = obstacle.gameObject;
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, dirToTarget, Color.red);
                }
            }
        }
    }

    // private Collider[] colliders = new Collider[30];
    // private Entity entity;
    // private RagdollController ragdollController;
    // private BehaviorGraphAgent behaviorGraphAgent;
    // [Header("시야 반경")]
    // public float radius = 10f;
    
    // private void Awake()
    // {
    //     entity = GetComponent<Entity>();
    //     ragdollController = GetComponent<RagdollController>();
    //     behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
    // }
    //
    // private void Start()
    // {
    //     StartCoroutine(SearchCoroutine());
    // }
    //
    // private WaitForSeconds wait = new WaitForSeconds(1f);
    // private IEnumerator SearchCoroutine()
    // {
    //     while (true)
    //     {
    //         if (ragdollController.ragdollEnabled)
    //         {
    //             enabled = false;
    //             yield break;
    //         }
    //
    //         Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
    //
    //         if (colliders != null)
    //         {
    //             foreach (Collider col in colliders)
    //             {
    //                 if (col == null) continue;
    //                 GameObject evidence = null;
    //                 if (col.GetComponentInParent<Entity>())
    //                 {
    //                     evidence = col.GetComponentInParent<Entity>().gameObject;
    //                 }
    //                 else if(col.GetComponentInParent<Obstacle>())
    //                 {
    //                     evidence = col.GetComponentInParent<Obstacle>().gameObject;
    //                 }
    //                 if(evidence == null) continue;
    //                 
    //                 if (evidence.CompareTag("Evidence"))
    //                 {
    //                     Vector3 dirToTarget = (evidence.transform.position - transform.position).normalized;
    //                     if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
    //                     {
    //                         float dst = Vector3.Distance(transform.position, evidence.transform.position);
    //                         if (Physics.Raycast(transform.position, dirToTarget, dst,
    //                                 1 << LayerMask.NameToLayer("Entity"))
    //                             || Physics.Raycast(transform.position, dirToTarget, dst,
    //                                 1 << LayerMask.NameToLayer("Obstacle")))
    //                         {
    //                             Debug.DrawRay(transform.position, dirToTarget, Color.red);
    //                             behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
    //                             if (alertTarget.Value != evidence.gameObject)
    //                             {
    //                                 behaviorGraphAgent.SetVariableValue("AlertTarget", evidence.gameObject);
    //                             }
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //
    //         yield return wait;
    //     }
    // }
}
