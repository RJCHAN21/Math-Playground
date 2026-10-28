using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private IHittable2D hitTarget;
    private PooledGun sourceGun;
    private PooledProjectile pooledProj;
    private Vector3 spawnPos;

    private void Awake()
    {
        pooledProj = GetComponent<PooledProjectile>();
    }

    private void Start()
    {
        spawnPos = transform.position;
    }

    private void Update()
    {
        Vector2 from = transform.position;
        transform.position += transform.right * speed * Time.deltaTime;
        Vector2 path = (Vector2)transform.position - from;

        if (hitTarget != null)
        {
            Vector2 center = hitTarget.HitPosition;
            float lengthSquared = path.sqrMagnitude;
            float t = lengthSquared > 0f
                ? Mathf.Clamp01(Vector2.Dot(center - from, path) / lengthSquared)
                : 0f;

            Vector2 closestPoint = from + path * t;
            float radius = hitTarget.HitRadius;

            if ((center - closestPoint).sqrMagnitude <= radius * radius)
            {
                hitTarget.Hit(sourceGun);
                pooledProj.ReturnToPool();
                return;
            }
        }
    }

    public void ResetSpawnPosition()
    {
        spawnPos = transform.position;
    }

    public void SetHitContext(IHittable2D target, PooledGun gun)
    {
        hitTarget = target;
        sourceGun = gun;
    }
}
