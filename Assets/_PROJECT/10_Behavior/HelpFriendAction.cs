using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HelpFriend", story: "Set [curHP] of [HelpTarget] to Max", category: "Action", id: "fdadf384245cc37d8a3fd09f3004dc7e")]
public partial class HelpFriendAction : Action
{
    [SerializeReference] public BlackboardVariable<string> curHP = new BlackboardVariable<string>("curHP");
    [SerializeReference] public BlackboardVariable<GameObject> HelpTarget;

    protected override Status OnStart()
    {
        if (HelpTarget.Value == null) return Status.Failure;
        
        BehaviorGraphAgent otherAgent = HelpTarget.Value.GetComponent<BehaviorGraphAgent>();
        if (otherAgent != null)
        {
            otherAgent.SetVariableValue(curHP.Value, 100f);
        }
        
        return Status.Success;
    }
}

