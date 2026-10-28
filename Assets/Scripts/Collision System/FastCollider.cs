public interface FastCollider
{
    ColliderBounds Bounds { get; }
    void NotifyCollision(FastCollider other) {}
}