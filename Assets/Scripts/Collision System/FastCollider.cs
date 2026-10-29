public interface FastCollider
{
    ColliderBounds Bounds { get; }
    public ColliderBounds PreviousBounds { get; set; }
    void NotifyCollision(FastCollider other) {}
    void ApplyXBounds(float minX, float maxX) {}
}