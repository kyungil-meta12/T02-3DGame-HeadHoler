using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OnStop", story: "Set [String] in [Animator] to [bool] On stop this", category: "Action", id: "6b0963fe9f37792689129560904cdc8f")]
public partial class OnStopAction : Action
{
    [SerializeReference] public BlackboardVariable<string> String;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<bool> Bool;
    //해당 분기 종료시 애니메이션 false

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if(String.Value == null || Animator.Value == null) return;
        
        Animator.Value.SetBool(String.Value, Bool.Value);
    }
}

