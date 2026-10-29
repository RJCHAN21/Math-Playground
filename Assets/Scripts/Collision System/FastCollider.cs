using UnityEngine;

public interface FastCollider
{
    ColliderBounds Bounds { get; }
    public ColliderBounds PreviousBounds { get; set; }
    void NotifyCollision(FastCollider other) {}
    public void ApplyBounds(Vector3 min, Vector3 max, Vector3 contactAxes) {}
}