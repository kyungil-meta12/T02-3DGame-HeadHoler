using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsTargetChange", story: "[Bool] Check if Fail when [Target] changes", category: "Modifer", id: "7b36a97d924bbdd6668dad7b082eb50a")]
public partial class IsTargetChangeAction : Modifier
{
    [SerializeReference] public BlackboardVariable<bool> Bool;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private GameObject postTarget;
    private bool isChanged;
    
    protected override Status OnStart()
    {
        if (Target.Value == null || Child == null)
        {
            return Status.Failure;
        }
        
        postTarget = Target.Value;
        
        Status status = StartNode(Child);
        if (status == Status.Success)
            return Status.Success;
        if (status == Status.Failure)
            return Status.Failure;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Status status = Child.CurrentStatus;
        if (status == Status.Success)
            return Status.Success;
        if (status == Status.Failure)
            return Status.Failure;
        
        if (postTarget == Target.Value)
        {
            if (Bool.Value)
            {
                return Status.Running;
            }
            else
            {
                EndNode(Child);
                return Status.Failure;
            }
        }
        else
        {
            postTarget = Target.Value;
            if (Bool.Value)
            {
                EndNode(Child);
                return Status.Failure;
            }
            else
            {
                return Status.Running;
            }
        }
    }
    
    protected override void OnEnd()
    {
        base.OnEnd();
        postTarget = null;
    }
}

