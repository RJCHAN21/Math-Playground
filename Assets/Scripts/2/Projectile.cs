using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float velocity = 10f;
    
    private Vector3 direction;

    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += direction * velocity * Time.deltaTime;
    }
}
