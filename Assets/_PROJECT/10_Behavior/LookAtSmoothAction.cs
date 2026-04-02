using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LookAtSmooth", story: "[Self] look at [Target] Smoothly [RotationSpeed]", category: "Action", id: "f79a66f1e59ba9dc3ea3b952cb305348")]
public partial class LookAtSmoothAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;

    private Vector3 direction;
    private Quaternion targetRotation;
    
    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            LogFailure($"Missing Self or Target.");
            return Status.Failure;
        }
        direction = Target.Value.transform.position - Self.Value.transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(direction);
        }

        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (Self.Value == null || Target.Value == null) return Status.Failure;                                            

        // 부드럽게 회전
        Self.Value.transform.rotation = Quaternion.Slerp(
            Self.Value.transform.rotation,
            targetRotation,
            Time.deltaTime * RotationSpeed.Value
        );

        // 각도 차이가 1도 이하면 완료
        float angleDiff = Quaternion.Angle(Self.Value.transform.rotation, targetRotation);
        if (angleDiff < 1f)
        {
            Self.Value.transform.rotation = targetRotation;                                             
            return Status.Success;
        }

        return Status.Running;
    }
}

