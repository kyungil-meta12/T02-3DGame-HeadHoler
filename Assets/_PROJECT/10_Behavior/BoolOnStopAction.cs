using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BoolOnStop", story: "Set [Bool1] to [Bool2] on stop this", category: "Action", id: "7f606246f888d0281b5de908addeabda")]
public partial class BoolOnStopAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Bool1;
    [SerializeReference] public BlackboardVariable<bool> Bool2;

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Bool1.Value = Bool2.Value;
    }
}

