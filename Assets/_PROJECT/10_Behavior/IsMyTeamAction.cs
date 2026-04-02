using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsMyTeam", story: "Are [AlertTarget] and [Self] on the same team?", category: "Action", id: "e2f1115f6b88050f95c217ed31bc304a")]
public partial class IsMyTeamAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> AlertTarget;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (AlertTarget.Value.GetComponentInParent<Entity>() == null)
        {
            AlertTarget.Value = null;
            return Status.Success;
        }
        if (AlertTarget.Value.GetComponentInParent<Entity>().myTeam != 
            Self.Value.GetComponentInParent<Entity>().myTeam)
        {
            AlertTarget.Value = null;
        }
        
        return Status.Success;
    }
}

