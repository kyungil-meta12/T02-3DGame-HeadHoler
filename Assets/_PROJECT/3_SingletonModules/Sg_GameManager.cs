using System;
using System.Collections.Generic;
using UnityEngine;

public class Sg_GameManager : MonoBehaviour
{
    public static Sg_GameManager Inst;
    public List<Entity> entities;

    private void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        print("[Sg_GameManager] Created instance.");
        entities = new List<Entity>();
    }

    void OnDestroy()
    {
        Inst = null;
    }
}
