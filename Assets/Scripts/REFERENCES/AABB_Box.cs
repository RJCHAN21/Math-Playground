using UnityEngine;

public class AABB_Box : MonoBehaviour
{
    public float width = 1;
    public float height = 1;
    public float depth = 1;
    public Color debugColor = Color.red;

    private AABB bounds;

    public AABB Bounds => bounds;

    private void Update()
    {
        bounds = new AABB(width, height, depth, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, depth));
    }

    public void NotifyCollision(AABB bounds)
    {
        Debug.Log("Notify Collision");
    }
}

[System.Serializable]
public struct AABB
{
    public Vector3 min,
        max;

    public AABB(float width, float height, float depth, Vector3 pos)
    {
        min = new Vector3(pos.x - width * .5f, pos.y - height * .5f, pos.z - depth * .5f);
        max = new Vector3(pos.x + width * .5f, pos.y + height * .5f, pos.z + depth * .5f);
    }
}