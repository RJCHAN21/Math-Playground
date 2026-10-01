using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class PooledGun : MonoBehaviour
{
    [SerializeField] private PooledProjectile projPrefab;
    [SerializeField] private UnityEvent gunFired;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    private IObjectPool<PooledProjectile> objPool;
    private float nextTimeToShoot;
    private ITurretFiringMode firingMode;
    private IHittable2D hitTarget;

    private void Awake()
    {
        objPool = new ObjectPool<PooledProjectile>(CreateProjectile,
            OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
            collectionCheck, defaultCapacity, maxSize);
    }

    private PooledProjectile CreateProjectile()
    {
        PooledProjectile projInstance = Instantiate(projPrefab);
        projInstance.ObjPool = objPool;
        return projInstance;
    }
    
    private void OnReleaseToPool(PooledProjectile pooledObj)
    {
        pooledObj.gameObject.SetActive(false);
    }

    private void OnGetFromPool(PooledProjectile pooledObj)
    {
        pooledObj.GetComponent<Projectile2D>().SetHitContext(hitTarget, this);
        pooledObj.gameObject.SetActive(true);
    }

    private void OnDestroyPooledObject(PooledProjectile pooledObj)
    {
        if (pooledObj == null) return;
        
        Destroy(pooledObj.gameObject);
    }

    public void SetFiringMode(ITurretFiringMode mode)
    {
        firingMode = mode;
    }
     
    public void Shoot()
    {
        if (Time.time < nextTimeToShoot) return;

        PooledProjectile projObj = objPool.Get();

        if (projObj == null)
            return;
        
        projObj.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
        projObj.Deactivate();

        nextTimeToShoot = Time.time + fireInterval;
        gunFired.Invoke();
    }

    public void ShootWithFiringMode()
    {
        if (firingMode == null)
        {
            Shoot();
            return;
        }

        firingMode.Fire(this);
    }

    public void FirePattern(float shotInterval, params float[] angleOffsets)
    {
        if (Time.time < nextTimeToShoot) return;

        int shotsFired = 0;

        foreach (float angleOffset in angleOffsets)
        {
            PooledProjectile projObj = objPool.Get();
            if (projObj == null) break;

            Quaternion shotRotation =
                muzzle.rotation * Quaternion.Euler(0f, 0f, angleOffset);

            projObj.transform.SetPositionAndRotation(muzzle.position, shotRotation);
            projObj.Deactivate();
            shotsFired++;
        }

        if (shotsFired == 0) return;

        nextTimeToShoot = Time.time + shotInterval;
        gunFired.Invoke();
    }

    public void SetHitTarget(IHittable2D target)
    {
        hitTarget = target;
    }
}
