using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NullCheck", story: "[Target] is Null [Bool]", category: "Conditions", id: "9a457f8479770eb2aea09f24c27caf27")]
public partial class NullCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    public override bool IsTrue()
    {
        bool result = Target.Value == null;
        return result == Bool.Value;
    }
}
