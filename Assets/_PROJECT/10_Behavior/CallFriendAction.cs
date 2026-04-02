using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CallFriend", story: "[Self] Call equal Entity to [AlertTarget] in [Radius]", category: "Action", id: "b3ec7c27886c1ae0db7a33da99262f84")]
public partial class CallFriendAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<GameObject> AlertTarget;

    protected override Status OnStart()
    {
        if (Self.Value == null) return Status.Failure;

        Entity myEntity = Self.Value.GetComponent<Entity>();
        
        if (myEntity == null) return Status.Failure;

        Collider[] colliders = Physics.OverlapSphere(Self.Value.transform.position, Radius.Value);

        foreach (var col in colliders)
        {
            if (col.GetComponentInParent<Entity>() == null) continue;
            if (col.GetComponentInParent<Entity>().gameObject == Self.Value || 
                col.CompareTag("Evidence")) continue;

            Entity otherEntity = col.GetComponent<Entity>();
            
            if (otherEntity != null)
            {
                if (otherEntity.myTeam == myEntity.myTeam)
                {
                    BehaviorGraphAgent otherAgent = otherEntity.GetComponent<BehaviorGraphAgent>();
                    if (otherAgent != null)
                    {
                        otherAgent.GetVariable<GameObject>("AlertTarget", out var alertTarget);
                        if (alertTarget.Value != AlertTarget.Value || alertTarget.Value != null)
                        {
                            alertTarget.Value = AlertTarget.Value;
                        }
                        //Debug.Log($"{myEntity.name}이(가) 동료 {otherEntity.name}을(를) 호출했습니다!");
                    }
                }
            }
        }

        return Status.Success;
    }
}

