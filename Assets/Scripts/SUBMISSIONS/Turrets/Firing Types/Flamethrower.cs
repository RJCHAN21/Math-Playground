using UnityEngine;

public sealed class Flamethrower : ITurretFiringMode
{
    private readonly float fullSpreadAngle;

    public Flamethrower(float fullSpreadAngle)
    {
        this.fullSpreadAngle = fullSpreadAngle;
    }
    
    public void Fire(PooledGun gun)
    {
        float halfSpread = fullSpreadAngle * 0.5f;

        gun.FirePattern(
            0.1f,
            Random.Range(-halfSpread, halfSpread),
            Random.Range(-halfSpread, halfSpread),
            Random.Range(-halfSpread, halfSpread));
    }
}
