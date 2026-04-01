using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Dead", story: "Disable [Self]", category: "Action", id: "8bdb49b7e3712c9ccb7c408b840672a6")]
public partial class DeadAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            //Debug.Log("Status Failure");
            return Status.Failure;
        }

        Entity myEntity = Self.Value.GetComponent<Entity>();
        
        if (myEntity != null)
        {
            myEntity.Die();
            
            //Debug.Log($"{Self.Value.name}의 AI가 완전히 정지되었습니다 (사망 처리).");
        }
        else
        {
            //Debug.Log("agent is null");
        }

        return Status.Success;
    }
}

