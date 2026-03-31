using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntilBoolsSame", story: "Wait until [Bool1] = [Bool2]", category: "Action", id: "f34fb4ee7c9a3e005b2a77b9df7f52fb")]
public partial class WaitUntilBoolsSameAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Bool1;
    [SerializeReference] public BlackboardVariable<bool> Bool2;

    protected override Status OnUpdate()
    {
        if (Bool1.Value == Bool2.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}

