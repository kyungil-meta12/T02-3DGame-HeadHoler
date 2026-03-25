using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Obstacle : MonoBehaviour
{
    /*
    public 필드 //인스펙터에서 관리해야 된다, 다른 스크립트가 호출해야 된다.
    
    private 필드 //자식이 몰라도 부모가 쓰면 된다.
    소리 감지영역 콜라이더
    
    메서드
    Hit() : 플레이어가 총을 레이캐스트로 쏘면 호출할 메서드
    OnColliderEnter(Collider other)
    콜라이더 크기 조절 코루틴

    알고리즘
    플레이어가 레이캐스트를 쏴서 맞았을때 Hit()호출
    소리 감지영역 콜라이더를 킨다.
    콜라이더 범위를 0부터 적절한 범위까지 적절한 시간 텀으로 늘려줄 코루틴을 호출한다.
    콜라이더 영역에 시민이나 적의 충돌을 감지한다.
    시민이나 적의 메서드를 호출한다. -> Character의 메서드를 호출한다.
    상속을 자주 쓰는 이유
    시민 - 농부, 목수, ... 종류를 여러가지 추가해도 Character만 상속하면 Obstacle과 상호작용가능.
    */

    protected virtual void Hit() //총알에 맞았을때
    {
        
    }

    protected virtual void OnColliderEnter(Collider other) //소리범위에 시민이나 적이 닿았을때 Character의 메서드를 호출한다.
    {
        
    }

    protected virtual IEnumerator SoundCoRoutine() //소리범위를 늘려준다.
    {
        yield return null;
    }

    protected virtual void UniqueInteraction() //고유한 작용
    {
        //깨진다
    }
}
