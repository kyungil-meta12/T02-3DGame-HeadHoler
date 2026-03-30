using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindNearFriend", story: "Find [NearFriend]", category: "Action", id: "4dc6bcabdf4918dea4f291b25e160034")]
public partial class FindNearFriendAction : Action
{
    //Sg_GameManager.entities중 가까운 동료 찾기
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> NearFriend = new BlackboardVariable<string>("NearFriend");

    protected override Status OnStart()
    {
        if (Self.Value == null || Sg_GameManager.Inst.entities == null) return Status.Failure;

        Entity myEntity = Self.Value.GetComponent<Entity>();
        if (myEntity == null) return Status.Failure;

        float minDistance = float.MaxValue;
        GameObject closestFriend = null;

        foreach (var ent in Sg_GameManager.Inst.entities)
        {
            if (ent.gameObject == Self.Value) continue;

            if (ent.myTeam == myEntity.myTeam)
            {
                float d = Vector3.Distance(Self.Value.transform.position, ent.transform.position);

                if (d < minDistance)
                {
                    minDistance = d;
                    closestFriend = ent.gameObject; 
                }
            }
        }

        if (closestFriend != null)
        {
            BehaviorGraphAgent myAgent = Self.Value.GetComponent<BehaviorGraphAgent>();
        
            if (myAgent != null)
            {
                myAgent.SetVariableValue(NearFriend.Value, closestFriend);
            
                // Debug.Log($"{myEntity.name}이(가) 가장 가까운 동료 {closestFriend.name}을(를) 찾았습니다! (거리: {minDistance})");
            }
        }

        return Status.Success;
    }
}

