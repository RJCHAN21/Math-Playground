using UnityEngine;

public interface CustomCollider
{
    ColliderBounds Bounds { get; }
    public ColliderBounds PreviousBounds { get; set; }
    bool IsTrigger { get; set; }
    bool IsCollideable { get; set; }
    bool IsActive { get; set; }
    void NotifyCollision(CustomCollider other) {}
    public void ApplyBounds(Vector3 min, Vector3 max, Vector3 contactAxes) {}
    void NotifyCollisionContact(CustomCollider other, Vector3 contactDirection);
    void NotifyTriggerEnter(CustomCollider other);
    void NotifyTriggerStay(CustomCollider other);
    void NotifyTriggerExit(CustomCollider other);
}