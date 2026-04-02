using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.GraphicsBuffer;

public class BoatController : MonoBehaviour
{
    public List<Transform> wayPoints = new();

    private Vector3 originPosition;
    private Quaternion originRotation;
    private int currentIndex;
    private MeshRenderer mr;
    private NavMeshAgent agent;

    void Awake()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        mr = GetComponentInChildren<MeshRenderer>();
        var mrRot = mr.transform.rotation.eulerAngles;
        mr.transform.rotation = Quaternion.Euler(mrRot.x, originRotation.eulerAngles.y - 90f, mrRot.z);
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.SetDestination(wayPoints[0].position);
    }

    void Update()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 5f)
        {
            SetDestinationToNextPoint();
        }

        var parentPos = transform.position;
        parentPos.y = originPosition.y;

        var parentRot = transform.rotation.eulerAngles.y;
        var childRot = mr.gameObject.transform.rotation.eulerAngles;
        childRot.y = parentRot - 90f;

        transform.position = parentPos;
        mr.gameObject.transform.rotation = Quaternion.Euler(childRot);
    }

    void SetDestinationToNextPoint()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }
        if (currentIndex < wayPoints.Count - 1)
        {
            agent.destination = wayPoints[currentIndex].position;
            currentIndex++;
        }
        else
        {
            transform.position = originPosition;
            transform.rotation = originRotation;
            var mrRot = mr.transform.rotation.eulerAngles;
            mr.transform.rotation = Quaternion.Euler(mrRot.x, originRotation.eulerAngles.y - 90f, mrRot.z);
            currentIndex = 0;
            agent.SetDestination(wayPoints[0].position);
            agent.Warp(originPosition);
        }
    }
}
