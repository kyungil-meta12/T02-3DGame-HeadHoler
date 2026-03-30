using System;
using System.Collections.Generic;
using UnityEngine;

public class Sg_GameManager : MonoBehaviour
{
    public static Sg_GameManager Inst;
    public static List<Entity> entities = new();

    private void Awake()
    {
        if(Inst && Inst != this)
        {
            DestroyImmediate(this);
            return;
        }
        Inst = this;
        print("[Sg_GameManager] Created instance.");
    }
}
