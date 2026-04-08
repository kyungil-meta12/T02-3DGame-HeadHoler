using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarIterator : MonoBehaviour
{
    public List<Transform> wayPoints = new();
    public Transform[] wheels;
    public float wheelRotationMultiplier = 500f; // 바퀴 회전 배수
    public bool stopWhenHit; // 총알에 맞으면 멈추는 여부
    private bool stopState = false; // 멈춤 여부

    private Vector3 originPosition;
    private Quaternion originRotation;
    private int currentIndex;
    private NavMeshAgent agent;

    void Start()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(wayPoints[0].position);
        agent.autoBraking = false;
        agent.isStopped = false;
    }

    void Update()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 3f)
        {
            SetDestinationToNextPoint();
        }

        float currentMoveSpeed = agent.velocity.magnitude;
        foreach (var wheel in wheels)
        {
            wheel.Rotate(Vector3.right * currentMoveSpeed * wheelRotationMultiplier * Time.deltaTime);
        }

        var currentPos = transform.position;
        currentPos.y = originPosition.y;
        transform.position = currentPos;

        // 멈춤상태가 되면 점차 속도를 줄인다
        if(stopState)
        {
            agent.speed -= Time.deltaTime * 5f;
            agent.speed = Mathf.Clamp(agent.speed, 0f, 100f);
        }
    }

    void SetDestinationToNextPoint()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }
        if (currentIndex < wayPoints.Count - 1)
        {
            currentIndex++;
            agent.destination = wayPoints[currentIndex].position;
        }
        else
        {
            transform.position = originPosition;
            transform.rotation = originRotation;
            currentIndex = 0;
            agent.Warp(originPosition);
            agent.SetDestination(wayPoints[0].position);
        }
    }

    //  차량 멈춤상태 활성화
    public void StopIteration()
    {
        stopState = stopWhenHit ? true : false;
    }
}
