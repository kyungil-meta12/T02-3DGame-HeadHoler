using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DeadCheck", story: "[Target] is Dead [Bool]", category: "Conditions", id: "b5fb056cb43d7cf893a051df6cc8d07e")]
public partial class DeadCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    public override bool IsTrue()
    {
        bool result = false;
        if (Target.Value != null)
        {
            var entity = Target.Value.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.isDead)
                {
                    result = true;
                    Target.Value = null;
                }
            }
        }
        
        return result == Bool.Value;
    }
    
}
