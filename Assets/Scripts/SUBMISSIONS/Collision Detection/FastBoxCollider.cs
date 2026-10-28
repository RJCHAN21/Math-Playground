using UnityEngine;

public class FastBoxCollider : MonoBehaviour, FastCollider
{
    [SerializeField] private Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] bool overrideCenter = false;
    [SerializeField] private Vector3 size = new Vector3(1, 1, 1);
    [SerializeField] private Color gizmosColor = Color.red;

    public ColliderBounds Bounds
    {
        get
        {
            Vector3 selectedPosition = overrideCenter ? center : transform.position;
            return new ColliderBounds(size, selectedPosition);
        }
    }

    private void Start()
    {
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Unregister(this);    
    }

    public void NotifyCollision(FastCollider other)
    {
        Debug.Log($"{gameObject.name} collided with {((Component)other).gameObject.name}");
    }

    private void OnDrawGizmos()
    {
        Vector3 selectedCenter = overrideCenter ? center : transform.position;
        CustomGizmos.DrawWireCube(selectedCenter, size, gizmosColor);
    }
}
