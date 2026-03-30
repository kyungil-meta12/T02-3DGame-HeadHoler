using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Escape", story: "Set the [furthestPoint] in [PatrolPoints] from [AlertTarget]", category: "Action", id: "368881791a50523f5a7952eec4f90833")]
public partial class EscapeAction : Action
{
    //경계대상에서 가장 먼 순찰 포인트 찾기
    [SerializeReference] public BlackboardVariable<GameObject> FurthestPoint;
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;
    [SerializeReference] public BlackboardVariable<GameObject> AlertTarget;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Self.Value == null || PatrolPoints ==  null || AlertTarget == null) return Status.Failure;

        float distance = Single.MinValue;
        GameObject furthestPoint = null;
        
        foreach (var point in PatrolPoints.Value)
        {
            float d = Vector3.Distance(point.transform.position, Self.Value.transform.position);
            if (d > distance)
            {
                distance = d;
                furthestPoint = point;
            }
        }

        if (furthestPoint != null)
        {
            FurthestPoint.Value = furthestPoint;
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }
}

