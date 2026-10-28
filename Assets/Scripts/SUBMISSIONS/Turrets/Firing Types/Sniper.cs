public sealed class Sniper : ITurretFiringMode
{
    public void Fire(PooledGun gun)
    {
        gun.FirePattern(1.5f, 0f);
    }
}