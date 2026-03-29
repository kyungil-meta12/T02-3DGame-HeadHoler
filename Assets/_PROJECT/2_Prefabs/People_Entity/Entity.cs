using System;
using UnityEngine;



public class Entity : MonoBehaviour
{
    public Team myTeam;

    private void Start()
    {
        Sg_GameManager.entities.Add(this);
    }

    private void OnDisable()
    {
        Sg_GameManager.entities.Remove(this);
    }
}
