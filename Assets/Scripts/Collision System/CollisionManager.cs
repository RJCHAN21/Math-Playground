using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }
    private List<FastCollider> colliders = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            FastCollider collider = colliders[i];
            for (int j = i + 1; j < colliders.Count; j++)
            {
                if (IsColliding(collider.Bounds, colliders[j].Bounds))
                {
                    collider.NotifyCollision(colliders[j]);
                }
            }
        }

        foreach (FastCollider collider in colliders)
        {
            collider.PreviousBounds = collider.Bounds;
        }
    }

    public void Register(FastCollider collider)
    {
        if (colliders.Contains(collider)) return;
        colliders.Add(collider);
    }

    public void Unregister(FastCollider collider)
    {
        if (!colliders.Contains(collider)) return;
        colliders.Remove(collider);
    }

    public bool IsColliding(ColliderBounds a, ColliderBounds b)
    {
        return a.min.x <= b.max.x
            && b.min.x <= a.max.x
            && a.min.y <= b.max.y
            && b.min.y <= a.max.y
            && a.min.z <= b.max.z
            && b.min.z <= a.max.z;
    }
}
