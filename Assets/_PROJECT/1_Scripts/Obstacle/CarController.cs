using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    public Transform[] wheels;
    public Transform[] waypoints;
    public float wheelRotationMultiplier = 500f; // 바퀴 회전 배수
    public float startTime;
    public float lightEnableTime;
    public GameObject carLight;

    private float currentTime;
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private float fixedHeight;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        carLight.SetActive(false);
        fixedHeight = transform.position.y;
    }

    void Update()
    {
        if (agent.isStopped) // 읿정 시간이 지나면 출발
        {
            currentTime += Time.deltaTime;
            if(currentTime >= lightEnableTime)
            {
                carLight.SetActive(true); // 출발 직전에 라이트 킴
            }
            if (currentTime >= startTime)
            {
                SetDestinationToNextPoint();
                agent.isStopped = false;
            }
        }
        else
        {
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
        }

        var position = transform.position;
        position.y = fixedHeight;
        transform.position = position;
    }

    void SetDestinationToNextPoint()
    {
        if (waypoints.Length == 0)
            return;
        if (currentIndex < waypoints.Length - 1)
        {
            currentIndex++;
            agent.destination = waypoints[currentIndex].position;
        }
    }
}