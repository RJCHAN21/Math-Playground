using UnityEngine;

public struct ColliderBounds
{
    public Vector3 min, max;

    public ColliderBounds(Vector3 size, Vector3 position)
    {
        float halfX = Mathf.Abs(size.x) / 2f;
        float halfY = Mathf.Abs(size.y) / 2f;
        float halfZ = Mathf.Abs(size.z) / 2f;

        min = new Vector3(position.x - halfX, position.y - halfY, position.z - halfZ);
        max = new Vector3(position.x + halfX, position.y + halfY, position.z + halfZ);
    }

}