using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PooledProjectile : MonoBehaviour
{
    [SerializeField] private float timeoutDelay = 3f;

    private IObjectPool<PooledProjectile> objPool;

    public IObjectPool<PooledProjectile> ObjPool { set => objPool = value; }

    private bool isReturned;
   
    public void Deactivate()
    {
        isReturned = false;
        GetComponent<Projectile2D>().ResetSpawnPosition();
        StartCoroutine(DeactivateRoutine(timeoutDelay));
    }

    IEnumerator DeactivateRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    public void ReturnToPool() 
    {
        if (isReturned) return;

        isReturned = true;
        StopAllCoroutines();
        objPool.Release(this);    
    }

}
