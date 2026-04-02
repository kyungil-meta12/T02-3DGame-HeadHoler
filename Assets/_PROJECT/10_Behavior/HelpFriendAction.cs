using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HelpFriend", story: "Set currentHP of [HelpTarget] to Max", category: "Action", id: "fdadf384245cc37d8a3fd09f3004dc7e")]
public partial class HelpFriendAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> HelpTarget;

    protected override Status OnStart()
    {
        if (HelpTarget.Value == null) return Status.Failure;
        
        Entity otherEntity = HelpTarget.Value.GetComponentInParent<Entity>();
        if (otherEntity != null)
        {
            otherEntity.currentHP = otherEntity.maxHP;
            otherEntity.GetComponentInParent<BehaviorGraphAgent>().SetVariableValue<bool>("isHurt", false);
        }
        
        return Status.Success;
    }
}

