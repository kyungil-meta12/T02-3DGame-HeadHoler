using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RequestHelp", story: "Set [HelpTargets] of [NearFriend] to [Self]", category: "Action", id: "bb883868b4ddeb47a0c9301db82ee25a")]
public partial class RequestHelpAction : Action
{
    [SerializeReference] public BlackboardVariable<string> HelpTargets = new BlackboardVariable<string>("HelpTargets");
    [SerializeReference] public BlackboardVariable<GameObject> NearFriend;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (NearFriend.Value == null || Self.Value == null) return Status.Failure;
        
        BehaviorGraphAgent otherAgent = NearFriend.Value.GetComponent<BehaviorGraphAgent>();
        if (otherAgent != null)
        {
            otherAgent.GetVariable<List<GameObject>>("HelpTargets", out var helpTargetsList);

            if (helpTargetsList != null)
            {
                // 2. 리스트에 자신이 이미 있는지 확인하여 중복 추가를 방지합니다.
                if (!helpTargetsList.Value.Contains(Self.Value))
                {
                    // 리스트에 자신을 추가합니다.
                    helpTargetsList.Value.Add(Self.Value);

                    // 3. 내부 리스트가 변경되었음을 Behavior 시스템에 알리기 위해 다시 Set 해줍니다.
                    otherAgent.SetVariableValue("HelpTargets", helpTargetsList);
                }
            }
        }

        return Status.Success;
    }
}

