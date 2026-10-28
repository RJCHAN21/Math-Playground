using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _width;

    public Vector2 max { get; private set; }
    public Vector2 min { get; private set; }

    private void Start()
    {
        if (CollisionManagerVariant.Instance != null) 
        {
            
            CollisionManagerVariant.Instance.Register(this);
        }
    }

    private void OnDestroy()
    {
        if (CollisionManagerVariant.Instance != null)
        {
            CollisionManagerVariant.Instance.Unregister(this);
        }
    }
    private void Update()
    {
        var posX = transform.position.x + _width * .5f;
        var posY = transform.position.y + _length * .5f;

        var negX = transform.position.x - _width * .5f;
        var negY = transform.position.y - _length * .5f;

        max = new Vector2(posX, posY);
        min = new Vector2(negX, negY);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(new Vector3(min.x, max.y), new Vector3(max.x, max.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(max.x, max.y), new Vector3(max.x, min.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(max.x, min.y), new Vector3(min.x, min.y), Color.blueViolet);
        Debug.DrawLine(new Vector3(min.x, min.y), new Vector3(min.x, max.y), Color.blueViolet);
    }

    public void OnCollision(AABBCollider other)
    {
        Debug.Log($"Hit {other.name}");
    }
}
