using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RequestHelp", story: "Set [HelpTarget] of [NearFriend] to [Self]", category: "Action", id: "bb883868b4ddeb47a0c9301db82ee25a")]
public partial class RequestHelpAction : Action
{
    [SerializeReference] public BlackboardVariable<string> HelpTarget = new BlackboardVariable<string>("HelpTarget");
    [SerializeReference] public BlackboardVariable<GameObject> NearFriend;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (NearFriend.Value == null || Self.Value == null) return Status.Failure;
        
        BehaviorGraphAgent otherAgent = NearFriend.Value.GetComponent<BehaviorGraphAgent>();
        if (otherAgent != null)
        {
            otherAgent.SetVariableValue(HelpTarget.Value, Self.Value);
        }

        return Status.Success;
    }
}

