using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class AirVentBase : Obstacle
{
	private Rigidbody rb;
	private bool isFalling = false;
	private bool isfalled = false;

	[Header("회전속도, 밀려나는 거리")]
	public float rotationSpeed = 2f;	//기본 1초에서 회전에 걸리는 시간 나눔값(2f = 0.5s)
	public float forwardPush = 1f;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void StartFalling()
	{
		if (isFalling == true)
		{
			return;
		}
		isFalling = true;

		if (rb != null)
		{
			StartCoroutine(FallingDown());

			//rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;   //Y좌표 외에 회전 포함 전부 잠금
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			rb.isKinematic = false;
			rb.useGravity = true;
		}
	}

	IEnumerator FallingDown()
	{
		Vector3 startPosition = transform.position;
		Vector3 endPosition = startPosition + (transform.forward * forwardPush);

		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = Quaternion.LookRotation(-transform.up, transform.forward);
		//Quaternion endRotation = startRotation * Quaternion.Eulur(90, 0, 0);

		float elapsed = 0f;
		while (elapsed < 1f)
		{
			elapsed += Time.deltaTime * rotationSpeed;

			transform.position = Vector3.Lerp(startPosition, endPosition, elapsed);
			transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed);

			yield return null;
		}

		transform.position = endPosition;
		transform.rotation = endRotation;
	}
	
	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		if (collision.gameObject.CompareTag("Entity"))
		{
			if (isfalled == false)
			{
				collision.gameObject.GetComponent<Entity>().Hit(
				collision.collider, transform.position - collision.contacts[0].point, damage);
			}
		}

			if (collision.gameObject.CompareTag("Ground"))
		{
			if (isfalled == false)
			{
				isfalled = true;
				Instantiate(hitSoundPrefab, transform.position, Quaternion.identity);
				Hit(transform.position);
			}
		}
	}
}
