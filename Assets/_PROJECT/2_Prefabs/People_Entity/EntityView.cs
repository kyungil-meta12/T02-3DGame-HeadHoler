
using System;
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

    private void Update()
    {
        if (ragdollController.ragdollEnabled)
        {
            enabled = false;
            return;
        }
        
        Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
        
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (col == null) continue;
                if (col.CompareTag("Evidence"))
                {
                    Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                    {
                        float dst = Vector3.Distance(transform.position, col.transform.position);
                        if (Physics.Raycast(transform.position, dirToTarget, dst,
                                1<<LayerMask.NameToLayer("Entity")) 
                            || Physics.Raycast(transform.position, dirToTarget, dst, 
                                1<<LayerMask.NameToLayer("Obstacle")))
                        {
                            Debug.DrawRay(transform.position, dirToTarget, Color.red);
                            behaviorGraphAgent.SetVariableValue("AlertTarget", col.transform.position);
                        }
                    }
                }
            }
        }
    }
}
