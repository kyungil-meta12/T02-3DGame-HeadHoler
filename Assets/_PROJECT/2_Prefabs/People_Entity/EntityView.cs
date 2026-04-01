
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EntityView : MonoBehaviour
{
    [Header("시야 반경")]
    public float radius = 10f;
    [Header("시야각")] 
    public float viewAngle = 90f;
    
    private Collider[] colliders = new Collider[30];

    private Entity entity;
    private RagdollController ragdollController;
    private BehaviorGraphAgent behaviorGraphAgent;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        ragdollController = GetComponent<RagdollController>();
        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
    }

    private void Start()
    {
        StartCoroutine(SearchCoroutine());
    }
    
    private WaitForSeconds wait = new WaitForSeconds(1f);
    private IEnumerator SearchCoroutine()
    {
        while (true)
        {
            if (ragdollController.ragdollEnabled)
            {
                enabled = false;
                yield break;
            }

            Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);

            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (col == null) continue;
                    GameObject evidence = null;
                    if (col.GetComponentInParent<Entity>())
                    {
                        evidence = col.GetComponentInParent<Entity>().gameObject;
                    }
                    else if(col.GetComponentInParent<Obstacle>())
                    {
                        evidence = col.GetComponentInParent<Obstacle>().gameObject;
                    }
                    if(evidence == null) continue;
                    
                    if (evidence.CompareTag("Evidence"))
                    {
                        Vector3 dirToTarget = (evidence.transform.position - transform.position).normalized;
                        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                        {
                            float dst = Vector3.Distance(transform.position, evidence.transform.position);
                            if (Physics.Raycast(transform.position, dirToTarget, dst,
                                    1 << LayerMask.NameToLayer("Entity"))
                                || Physics.Raycast(transform.position, dirToTarget, dst,
                                    1 << LayerMask.NameToLayer("Obstacle")))
                            {
                                Debug.DrawRay(transform.position, dirToTarget, Color.red);
                                behaviorGraphAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
                                if (alertTarget.Value != evidence.gameObject)
                                {
                                    behaviorGraphAgent.SetVariableValue("AlertTarget", evidence.gameObject);
                                }
                            }
                        }
                    }
                }
            }

            yield return wait;
        }
    }
}
