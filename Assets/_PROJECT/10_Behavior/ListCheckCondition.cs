using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ListCheck", story: "[List] is empty [Bool]", category: "Conditions", id: "80266cf18436e5c198ffd8531ab60019")]
public partial class ListCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    public override bool IsTrue()
    {
        if (List.Value == null)
        {
            return false;
        }
        if (List.Value.Count == 0)
        {
            return Bool.Value;
        }
        return !Bool.Value;
    }
}
