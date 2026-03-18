using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀 유틸
// 각 프리펩 타입에 맞게 인스턴스를 얻거나 반환
// 예:
// var inst = St_ObjectPool.Inst.Get(prefab);
// St_ObjectPool.Inst.Return(inst);

public class St_ObjectPool : MonoBehaviour
{
    public static St_ObjectPool Inst;
    private Dictionary<GameObject, List<GameObject>> poolDict = new Dictionary<GameObject, List<GameObject>>();

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        print($"[{typeof(St_ObjectPool)}] Created instance.");
    }

    void OnDestroy()
    {
        Inst = null;        
    }

    public T Get<T>(T prefab) where T : Component
    {
        GameObject prefabKey = prefab.gameObject;

        if (!poolDict.ContainsKey(prefabKey))
        {
            poolDict[prefabKey] = new List<GameObject>();
        }

        List<GameObject> pool = poolDict[prefabKey];
        GameObject instObj;

        while (pool.Count > 0)
        {
            instObj = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);

            if (instObj != null)
            {
                instObj.SetActive(true);
                return instObj.GetComponent<T>();
            }
        }

        instObj = Instantiate(prefabKey);
        return instObj.GetComponent<T>();
    }


    public void Return<T>(T prefab, T instance) where T : Component
    {
        if (instance == null) return;

        GameObject prefabKey = prefab.gameObject;
        
        if (!poolDict.ContainsKey(prefabKey))
        {
            poolDict[prefabKey] = new List<GameObject>();
        }

        instance.gameObject.SetActive(false);

        if (!poolDict[prefabKey].Contains(instance.gameObject))
        {
            poolDict[prefabKey].Add(instance.gameObject);
        }
    }
}