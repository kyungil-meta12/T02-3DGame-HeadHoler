using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForNull", story: "Wait for [Target] is Null [Bool]", category: "Action", id: "e7952190ff3d11783d5cb4b33f51c49c")]
public partial class WaitForNullAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    protected override Status OnUpdate()
    {
        if (Bool.Value)
        {
            if (Target.Value == null) return Status.Success;
        }
        else
        {
            if (Target.Value != null) return Status.Success;
        }
        
        return Status.Running;
    }
}

