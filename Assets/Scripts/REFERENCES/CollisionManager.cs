using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private List<AABB_Box> bounds = new();

    private void Start()
    {
        bounds = FindObjectsByType<AABB_Box>().ToList();
    }

    private void Update()
    {
        for (int i = 0; i < bounds.Count; i++)
        {
            AABB_Box box = bounds[i];
            for (int j = 0; j < bounds.Count; j++)
            {
                if (j == i)
                    continue;

                if (IsColliding(box.Bounds, bounds[j].Bounds))
                {
                    box.NotifyCollision(bounds[j].Bounds);
                }
            }
        }
    }

    public bool IsColliding(AABB a, AABB b)
    {
        return a.min.x <= b.max.x
            && b.min.x <= a.max.x
            && a.min.y <= b.max.y
            && b.min.y <= a.max.y
            && a.min.z <= b.max.z
            && b.min.z <= a.max.z;
    }
}
