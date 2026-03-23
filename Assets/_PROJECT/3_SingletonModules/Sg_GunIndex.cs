using UnityEngine;

// 플레이어 총 선택 인덱스를 관리하는 모듈.
// 메인 타이틀 씬에서 부터 미리 인스턴스를 만들어두고 사용하는 것이 좋다.
// 총기 선택 씬 -> 게임 씬 진입 시 이 모듈이 가진 값으로 플레이어 객체가 총기 인덱스를 선택하여 활성화 한다.
public class Sg_GunIndex : MonoBehaviour
{
    public static Sg_GunIndex Inst;
    public int currIndex = 0; // 기본값은 0이지만 개발 시에 인스펙터에서 다른 값으로 변경해서 테스트 가능

    void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        print("[Sg_GunSelector] Created instance.");
    }

    // 총기 선택 화면에서 인덱스를 선택하여 현재 선택된 총기를 변경한다.
    public void SelectIndex(int idx)
    {
        currIndex = idx;    
    }

    // 현재 인덱스를 리턴한다. 
    public int GetCurrentIndex()
    {
        return currIndex;
    }
}
