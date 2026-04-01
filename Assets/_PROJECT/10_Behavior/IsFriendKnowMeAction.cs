using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsFriendKnowMe", story: "Is [NearFriend] Know [Self]", category: "Action", id: "ab7c289dfe532e8fefec886a3d680011")]
public partial class IsFriendKnowMeAction : Action
{
    //NearFriend의 HelpTargets리스트에 내가 있는지 검사
    [SerializeReference] public BlackboardVariable<GameObject> NearFriend;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (NearFriend.Value == null || Self.Value == null) return Status.Failure;
        
        BehaviorGraphAgent otherAgent = NearFriend.Value.GetComponent<BehaviorGraphAgent>();
        if (otherAgent != null)
        {
            otherAgent.GetVariable<List<GameObject>>("HelpTargets", out var helpTargetsList);

            if (helpTargetsList != null && helpTargetsList.Value.Contains(Self.Value))
            {
                return Status.Success;
            }
        }
        return Status.Failure;
    }
}

