using UnityEngine;

/// <summary>
/// Math-based 3D Object Detection for Unity
/// </summary>
public static class Object3DDetector
{
    /// <summary>
    /// Checks for targets in a field-of-view arc.
    /// </summary>
    /// <param name="origin">The object to scan for targets.</param>
    /// <param name="target">The target to search for in a field-of-view arc.</param>
    /// <param name="range">How far the field-of-view arc extends.</param>
    /// <param name="coneAngle">The total angle, in degrees, of the field-of-view arc.</param>
    /// <returns>True if the target is inside the cone; otherwise, false.</returns>
    public static bool IsInCone(
        Transform origin,
        Transform target,
        float range,
        float coneAngle)
    {
        Vector3 dir = target.position - origin.position;

        if (dir.magnitude > range)
            return false;
        
        float delta = Vector3.Angle(origin.forward, dir);

        return delta <= coneAngle / 2f;
    }
}
