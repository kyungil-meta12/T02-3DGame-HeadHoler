using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolPointer", story: "Select [PatrolPoint] in [PatrolPoints]", category: "Action", id: "a4a926058fc7419af748aaa465e72175")]
public partial class PatrolPointerAction : Action
{
    //순찰 포인트 바꾸기
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PatrolPoint;
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;

    protected override Status OnStart()
    {
        if (Self.Value == null || PatrolPoints ==  null) return Status.Failure;

        int currentIndex = 0;
            
        if (PatrolPoints.Value.Contains(PatrolPoint.Value))
        {
            currentIndex = PatrolPoints.Value.IndexOf(PatrolPoint.Value);
        }

        int nextIndex = (currentIndex + 1) % PatrolPoints.Value.Count;
        
        PatrolPoint.Value = PatrolPoints.Value[nextIndex];
        
        return Status.Success;
    }
}

