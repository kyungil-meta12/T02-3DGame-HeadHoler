
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;

public class EntityView : MonoBehaviour
{
    [Header("시야각")] 
    public float viewAngle = 90f;

    private Collider col;
    private Entity myEntity;
    private BehaviorGraphAgent behaviorGraphAgent;
    private BlackboardVariable<GameObject> alertTarget;
    private BlackboardVariable<List<GameObject>> fracturedTargets;
    private HashSet<GameObject> targetSet;
    private List<Evidence> evidences = new List<Evidence>();

    private void Awake()
    {
        col = GetComponent<Collider>();
        myEntity = GetComponentInParent<Entity>();
        behaviorGraphAgent = GetComponentInParent<BehaviorGraphAgent>();
        behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out alertTarget);
        behaviorGraphAgent.GetVariable<List<GameObject>>("FracturedTargets", out fracturedTargets);
        targetSet = new HashSet<GameObject>(fracturedTargets.Value);
    }

    public float checkInterval = 0.5f; 
    private float nextCheckTime = 0f; 
    private void OnTriggerStay(Collider other)
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval; 
        }
        else return;
        //시체, 증거물 태그 검사
        if (other.gameObject.CompareTag("Evidence") || other.gameObject.CompareTag("FracturedObject"))
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
                    //부모가 evidence인지 검사
                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
                    Evidence evidence = other.gameObject.GetComponentInParent<Evidence>();
                    if (evidence != null)
                    {
                        if (!evidences.Contains(evidence))
                        {
                            alertTarget.Value = evidence.gameObject;
                            evidences.Add(evidence);
                        }
                        //AlertTarget이 동일한지 검사
                        if (alertTarget.Value == null)
                        {
                            alertTarget.Value = evidence.gameObject;
                        }
                    }
                }
                else if (isHitObstacle)
                {
                    Debug.DrawRay(transform.position, dirToTarget, Color.green);
                    
                    //FracturedTargets 리스트에 있는지 검사
                    if (!targetSet.Contains(other.gameObject))
                    {
                        targetSet.Add(other.gameObject);
                        behaviorGraphAgent.SetVariableValue<List<GameObject>>("FracturedTargets", targetSet.ToList());
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
