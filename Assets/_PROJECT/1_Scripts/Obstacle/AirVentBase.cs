using System.Collections;
using UnityEngine;

public class AirVentBase : MonoBehaviour
{
	private Rigidbody rb;
	private bool isFalling = false;

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
			rb.isKinematic = false;
			rb.useGravity = true;

			rb.constraints = RigidbodyConstraints.FreezeRotation;   //물리회전 잠금

			

			StartCoroutine(FallingDown());
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
}
