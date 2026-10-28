using UnityEngine;
using System.Collections.Generic;

public class CollisionManagerVariant : MonoBehaviour
{
    public static CollisionManagerVariant Instance => instance;
    private static CollisionManagerVariant instance;
    private List<AABBCollider> colliders = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    } 

    public void Register(AABBCollider collider)
    {
        if (colliders.Contains(collider)) return;
        colliders.Add(collider);
    }

    public void Unregister(AABBCollider collider)
    {
        if (!colliders.Contains(collider)) return;
        colliders.Remove(collider);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            var colliderA = colliders[i];
            for (int j = 0; j < colliders.Count; j++)
            {
                if (i == j) continue;
                var colliderB = colliders[j];
                if (Overlaps(colliderA, colliderB))
                {
                    colliderA.OnCollision(colliderB);
                    colliderB.OnCollision(colliderA);
                }
            }
        }
    }

    private bool Overlaps(AABBCollider a, AABBCollider b)
    {
        return a.min.x <= b.max.y && b.min.x <= a.max.x &&
            a.min.y <= b.max.y && b.min.y <= a.max.y;
    } 
}
