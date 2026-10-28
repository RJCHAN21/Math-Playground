using UnityEngine;

public interface IHittable2D
{
    Vector2 HitPosition { get; }
    float HitRadius { get; }

    void Hit(PooledGun sourceGun);
}
