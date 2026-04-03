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

    public GameObject flameParticle;
    public GameObject explosionParticle;

    private Rigidbody[] rigidWheels;
    private Rigidbody body;

    private float currentTime;
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private float fixedHeight;
    private bool fireStarted;
    private float explosionTime;
    private bool exploded;

    private Vector3 originFlameRotation;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.autoBraking = false;
        carLight.SetActive(false);
        fixedHeight = transform.position.y;

        body = GetComponentInChildren<Rigidbody>();
        rigidWheels = GetComponentsInChildren<Rigidbody>();
        foreach (var wheel in rigidWheels)
        {
            wheel.isKinematic = true; // 튐 방지를 위해 일시적으로 비활성화
        }
        body.isKinematic = true;

        originFlameRotation = flameParticle.transform.rotation.eulerAngles;
    }

    void Update()
    {
        if (agent.enabled)
        {
            if (agent.isStopped) // 일정 시간이 지나면 출발
            {
                currentTime += Time.deltaTime;
                if (currentTime >= lightEnableTime)
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
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    SetDestinationToNextPoint();
                }
            }

            // 실제 이동 속도에 비례한 바퀴 회전
            float currentMoveSpeed = agent.velocity.magnitude;
            foreach (var wheel in wheels)
            {
                wheel.Rotate(Vector3.right * currentMoveSpeed * wheelRotationMultiplier * Time.deltaTime);
            }

            var position = transform.position;
            position.y = fixedHeight;
            transform.position = position;
        }

        else
        { // 차가 구르다가 멈추면 엔진룸에서 불이 나기 시작한다
            if(body.linearVelocity.magnitude <= 1f)
            {
                flameParticle.SetActive(true);
                fireStarted = true;
            }

            if(fireStarted)
            {
                flameParticle.transform.rotation = Quaternion.Euler(originFlameRotation); // 불이 차체의 방향을 따라가지 않고 항상 위를 바라보도록 한다

                explosionTime += Time.deltaTime;
                if(!exploded && explosionTime >= 4f) // 불이 붙은 후 4초가 지나면 폭발
                {
                    explosionParticle.SetActive(true);
                    explosionParticle.transform.position = body.position;
                    carLight.SetActive(false);

                    Renderer ren = body.gameObject.GetComponent<Renderer>();
                    Material[] sharedMats = ren.materials;
                    for (int i = 0; i < sharedMats.Length; i++)
                    {
                        sharedMats[i].color = Color.black;
                    }
                    ren.materials = sharedMats;

                    // 폭발의 역동성을 위해 의도적으로 토크를 가한다
                    body.AddForce(Vector3.up * 180f, ForceMode.Impulse); // 폭발하며 자체가 튀어오른다
                    body.AddTorque(Vector3.forward * 180f, ForceMode.Impulse);
                    foreach(var wheel in rigidWheels) // 바퀴도 빠지며 날아간다
                    {
                        var joint = wheel.GetComponent<ConfigurableJoint>();
                        if(joint)
                        {
                            joint.connectedBody = null;
                            Destroy(joint);
                        }
                        wheel.AddForce((wheel.position - body.position) * 100f, ForceMode.Impulse);
                    }
                    exploded = true;
                }
                else if(explosionTime >= 8f) // 폭발 파티클을 다시 비활성화
                {
                    explosionParticle.SetActive(false);
                }
            }
        }
    }

    void SetDestinationToNextPoint()
    {
        if (waypoints.Length == 0)
            return;
        if (currentIndex < waypoints.Length - 1)
        {
            agent.destination = waypoints[currentIndex].position;
            currentIndex++;
        }
        if (currentIndex == waypoints.Length - 1) // 마지막 지점이라면 오토브레이킹 활성화
        {
            agent.autoBraking = true;
        }
    }

    public void SetCarDamaged()
    {
        if (!agent.enabled) // 한 번 파괴상태로 전환하면 중복 실행을 하지 않는다
        {
            return;
        }

        foreach(var wheels in rigidWheels)
        {
            wheels.isKinematic = false;
        }
        body.isKinematic = false;
        body.linearVelocity = agent.velocity;
        agent.enabled = false;
    }
}