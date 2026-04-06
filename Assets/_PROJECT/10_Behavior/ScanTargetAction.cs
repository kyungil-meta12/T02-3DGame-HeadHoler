using System;
using DinoFracture;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ScanTarget", story: "[Self] Scan [Target]", category: "Action", id: "0bd4676a0ee54ed3f809caaa5bd057be")]
public partial class ScanTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Target.Value == null || Self.Value == null) return Status.Failure;

        Entity targetEntity = Target.Value.GetComponentInParent<Entity>();
        RagdollController targetRagdollController = Target.Value.GetComponentInParent<RagdollController>();
        Obstacle obstacle = Target.Value.GetComponentInParent<Obstacle>();
        HitSound hitCol = Target.Value.GetComponentInChildren<HitSound>();
        FracturedObject fracturedObject = Target.Value.GetComponentInChildren<FracturedObject>();

        if (targetEntity != null)
        {
            if (targetEntity.myTeam == Self.Value.GetComponent<Entity>().myTeam)
            {
                if(hitCol != null)
                {
                    if (hitCol.isGunShot)
                    {
                        //총 맞은 흔적이 있으면 저격 발각, 게임오버
                        Sg_GameManager.Inst.GameOver();
                    }
                }
            }
            else
            {
                Target.Value = null;
                return Status.Success;
            }
        }

        foreach (var entity in Sg_GameManager.Inst.entities)
        {
            if(entity == Self.Value.GetComponent<Entity>()) continue;
            
            entity.GetComponent<BehaviorGraphAgent>().GetVariable<GameObject>("AlertTarget", out var alertTarget);
            if (alertTarget.Value == Target.Value)
            {
                //다른 Entity의 AlertTarget이 스캔 완료된 타겟과 같으면 제거완료 처리
                alertTarget.Value = null;
            }
        }
        //해당흔적 제거완료 처리
        if (hitCol != null) hitCol.ScanComplete();
        if (fracturedObject != null) fracturedObject.ScanComplete();
        if (obstacle != null) obstacle.ScanComplete();
        if (targetRagdollController != null)
        {
            if (targetRagdollController.ragdollEnabled && targetEntity != null)targetEntity.ScanComplete();
        }
        
        return Status.Success;
    }
}

