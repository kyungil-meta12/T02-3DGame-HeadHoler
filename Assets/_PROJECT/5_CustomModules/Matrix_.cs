using UnityEngine;

// 4x4 행렬 모듈
// 기존의 transform으로 표현하기 어려운 연산의 경우 해당 모듈을 활용한다.
// 변환 이전에 반드시 Identity()로 행렬을 초기화해야한다.
// 연산 순서에 따라 결과가 달라짐에 유의해야한다.
//  Translate -> Rotate != Rotate -> Translate

public class Matrix_
{
    Matrix4x4 M;

    public void Identity()
    {
        M = Matrix4x4.identity;
    }

    public void Translate(Vector3 translate)
    {
        M *= Matrix4x4.Translate(translate);
    }

    public void Rotate(Vector3 rotation)
    {
        M *= Matrix4x4.Rotate(Quaternion.Euler(rotation));
    }

    public void Scale(Vector3 scale)
    {
        M *= Matrix4x4.Scale(scale);
    }

    public void Dispatch(Transform ObjectTransform)
    {
        ObjectTransform.position = M.GetColumn(3);
        ObjectTransform.rotation = M.rotation; 
        ObjectTransform.localScale = new Vector3(
            M.GetColumn(0).magnitude,
            M.GetColumn(1).magnitude,
            M.GetColumn(2).magnitude
        );
    }
}