using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    public Transform[] wheels;
    public Transform[] waypoints;
    public float moveSpeed = 5f; // 자동차 이동 속도
    public float wheelRotationMultiplier = 500f; // 바퀴 회전 배수
    public bool rolling = true;

    private int currentIndex = 0;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = 720f;
        agent.acceleration = 100f;
    }

    void Start()
    {
        if (rolling && waypoints.Length > 0)
        {
            SetDestinationToNextPoint();
        }
    }

    void Update()
    {
        if (!rolling || currentIndex == waypoints.Length - 1)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        // 도착 확인 및 다음 지점 갱신
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetDestinationToNextPoint();
        }

        // 실제 이동 속도에 비례한 바퀴 회전
        float currentMoveSpeed = agent.velocity.magnitude;
        foreach (var wheel in wheels)
        {
            wheel.Rotate(Vector3.right * currentMoveSpeed * wheelRotationMultiplier * Time.deltaTime);
        }
        var position = transform.position;
        position.y = 5.53f;
        transform.position = position;
    }

    void SetDestinationToNextPoint()
    {
        if (waypoints.Length == 0)
            return;

        agent.destination = waypoints[currentIndex].position;
        currentIndex = (currentIndex + 1) % waypoints.Length;
    }
}