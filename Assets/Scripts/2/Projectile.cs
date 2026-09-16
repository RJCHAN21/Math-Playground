using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDist = 20f;

    private Vector3 spawnPos;

    private void Start()
    {
        spawnPos = transform.position;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if ((transform.position - spawnPos).sqrMagnitude >= maxDist * maxDist)
        {
            Destroy(gameObject);
        }
    }
}
