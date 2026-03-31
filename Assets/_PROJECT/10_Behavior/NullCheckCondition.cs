using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NullCheck", story: "[Target] is Null", category: "Conditions", id: "9a457f8479770eb2aea09f24c27caf27")]
public partial class NullCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override bool IsTrue()
    {
        return Target.Value == null;
    }
}
