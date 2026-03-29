using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CallFriend", story: "Call equal Entity to [Self] in [Radius]", category: "Action", id: "b3ec7c27886c1ae0db7a33da99262f84")]
public partial class CallFriendAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Radius;
    // 동료의 블랙보드에 있는 'Target' 변수 이름 (기본값 세팅)
    [SerializeReference] public BlackboardVariable<string> Target = new BlackboardVariable<string>("AlertTarget");

    protected override Status OnStart()
    {
        // 1. 예외 처리: 내 오브젝트가 없으면 실패
        if (Self.Value == null) return Status.Failure;

        // 2. 내 Entity 컴포넌트 가져오기
        Entity myEntity = Self.Value.GetComponent<Entity>();
        if (myEntity == null) return Status.Failure;

        // 3. 내 주변 반경(Radius) 안에 있는 모든 콜라이더 탐색
        Collider[] colliders = Physics.OverlapSphere(Self.Value.transform.position, Radius.Value);

        foreach (var col in colliders)
        {
            // 나 자신은 부를 필요 없으니 건너뜀
            if (col.gameObject == Self.Value) continue;

            // 4. 주변 물체에서 Entity 컴포넌트 찾기
            Entity otherEntity = col.GetComponent<Entity>();
            
            if (otherEntity != null)
            {
                // 5. 핵심 로직: 나와 완전히 같은 Team인지 확인! 
                // (Citizen은 Citizen만, Enemy는 Enemy만 통과됨)
                if (otherEntity.myTeam == myEntity.myTeam)
                {
                    // 6. 같은 동료라면? 동료의 BehaviorAgent를 찾아서 내 위치(또는 나)를 타겟으로 지정
                    BehaviorGraphAgent otherAgent = otherEntity.GetComponent<BehaviorGraphAgent>();
                    if (otherAgent != null)
                    {
                        // 동료의 블랙보드 'AlertTarget' 변수에 나 자신(Self)을 집어넣어 줌!
                        // (만약 타겟을 나 자신이 아닌 '시체'로 넘겨주고 싶다면 이 부분을 수정하면 됩니다)
                        otherAgent.BlackboardReference.SetVariableValue(Target.Value, Self.Value);
                        
                        //Debug.Log($"{myEntity.name}이(가) 동료 {otherEntity.name}을(를) 호출했습니다!");
                    }
                }
            }
        }

        // 주변 탐색 및 호출이 무사히 끝났으므로 성공 반환
        return Status.Success;
    }
}

