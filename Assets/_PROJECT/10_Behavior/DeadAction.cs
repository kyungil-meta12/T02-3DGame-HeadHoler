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
        if (Self.Value == null) return Status.Failure;

        // 1. 내 오브젝트에서 BehaviorGraphAgent 컴포넌트 찾기
        BehaviorGraphAgent agent = Self.Value.GetComponent<BehaviorGraphAgent>();
        
        if (agent != null)
        {
            // 2. 인스펙터 창의 체크박스를 해제하는 것과 똑같은 기능!
            agent.enabled = false;

            var regController = agent.gameObject.GetComponent<RagdollController>();
            regController.EnableRagdoll();
            //Debug.Log($"{Self.Value.name}의 AI가 완전히 정지되었습니다 (사망 처리).");
        }

        return Status.Success;
    }
}

