using System;
using System.Collections;
using UnityEngine;



public class Entity : MonoBehaviour
{
    private static readonly int AlertLevel = Animator.StringToHash("AlertLevel");
    
    [Header("소속")]
    public Team myTeam;
    [Header("경계 지속시간")]
    public static float alertTimer = 5f;

    internal int alertLevel;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Sg_GameManager.entities.Add(this);
        //StartCoroutine(AlertTimer());
    }

    private void Update()
    {
        //animator.SetInteger(AlertLevel, alertLevel);
    }

    private void OnDisable()
    {
        Sg_GameManager.entities.Remove(this);
    }
    
    private WaitForSeconds alertWait = new WaitForSeconds(alertTimer);

    private IEnumerator AlertTimer()
    {
        while (true)
        {
            yield return alertWait;
            alertLevel -= 1;
            animator.SetInteger(AlertLevel, alertLevel);
        }
    }
}
