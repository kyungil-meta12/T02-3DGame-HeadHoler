using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private ParticleSystem particle;
    private float currentTime;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        particle.Play();
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= 2.5f)
        {
            Destroy(gameObject);
        }
    }
}
