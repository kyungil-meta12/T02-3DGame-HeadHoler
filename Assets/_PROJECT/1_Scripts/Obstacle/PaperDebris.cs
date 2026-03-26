using System.Collections;
using UnityEngine;

public class PaperDebris : MonoBehaviour
{
	//종이 낙하까지 기다린 후 고정
	void Start()
    {
		StartCoroutine(FreezeAfterTime(7.0f));
    }

    IEnumerator FreezeAfterTime(float delay)
	{
		yield return new WaitForSeconds(delay); //생성되고 일정시간동안 기다림

		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true;	//일정시간 후 고정
		}

	}
}
