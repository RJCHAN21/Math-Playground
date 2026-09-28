using UnityEngine;

/// <summary>
/// Math-based 2D Object Detection for Unity
/// </summary>
public static class SightDetector2D
{
    /// <summary>
    /// Checks for targets in a field-of-view arc.
    /// </summary>
    public static bool IsInCone(
        Transform origin,
        Transform target,
        float range,
        float coneAngle)
    {
        Vector2 dir = target.position - origin.position;

        if (dir.magnitude > range) 
            return false;
        
        float pAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float tAngle = origin.eulerAngles.z;
        float delta = Mathf.Abs(Mathf.DeltaAngle(tAngle, pAngle));

        return delta <= coneAngle/2f;
    }
    /// <summary> 
    /// Detects a target whose center is inside a narrow strip ahead of the originating object.
    /// </summary>
    public static bool IsInLine(
        Transform origin,
        Transform target,
        float range,
        float halfWidth)
    {
        Vector2 toTarget = target.position - origin.position;

        float forwardDistance = Vector2.Dot(toTarget, origin.right);
        if (forwardDistance < 0f || forwardDistance > range)
            return false;

        float sidewaysDistance = Mathf.Abs(Vector2.Dot(toTarget, origin.up));
        return sidewaysDistance <= halfWidth;
    }
}