using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindNearFriend", story: "[Self] Find [NearFriend]", category: "Action", id: "4dc6bcabdf4918dea4f291b25e160034")]
public partial class FindNearFriendAction : Action
{
    //Sg_GameManager.entities중 가까운 동료 찾기
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> NearFriend;

    protected override Status OnStart()
    {
        if (Self.Value == null || Sg_GameManager.Inst.entities == null) return Status.Failure;

        Entity myEntity = Self.Value.GetComponent<Entity>();
        if (myEntity == null) return Status.Failure;

        float minDistance = float.MaxValue;
        GameObject closestFriend = null;
        BehaviorGraphAgent myAgent = Self.Value.GetComponent<BehaviorGraphAgent>();
        
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
            if (myAgent != null)
            {
                NearFriend.Value = closestFriend;
                closestFriend.GetComponentInParent<BehaviorGraphAgent>().GetVariable<List<GameObject>>(
                    "HelpTargets", out var helpTargets);
                closestFriend.GetComponentInParent<BehaviorGraphAgent>().GetVariable<GameObject>(
                    "HelpTarget", out var helpTarget);
                if (!helpTargets.Value.Contains(Self.Value))
                {
                    helpTargets.Value.Add(Self.Value);
                    if (helpTarget.Value == null)
                    {
                        helpTarget.Value = Self.Value;
                    }
                }
                
                // Debug.Log($"{myEntity.name}이(가) 가장 가까운 동료 {closestFriend.name}을(를) 찾았습니다! (거리: {minDistance})");
            }
        }

        return Status.Success;
    }
}

