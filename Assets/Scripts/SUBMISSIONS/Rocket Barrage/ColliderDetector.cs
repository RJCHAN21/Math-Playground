using UnityEngine;

/// <summary>
/// Lightweight manual collision checkers w/o Unity Physics.
/// </summary>
public static class ColliderDetector
{
    /// <summary>
    /// Checks whether two spherical areas overlap.
    /// Uses squared distance to avoid calculating a square root.
    /// </summary>
    /// <param name="positionA">Center position of the first sphere.</param>
    /// <param name="radiusA">Radius of the first sphere.</param>
    /// <param name="positionB">Center position of the second sphere.</param>
    /// <param name="radiusB">Radius of the second sphere.</param>
    /// <returns>True when the two spherical areas overlap.</returns>
    public static bool SphereOverlap(
        Vector3 positionA,
        float radiusA,
        Vector3 positionB,
        float radiusB)
    {
        float combinedRadius = radiusA + radiusB;

        return (positionB - positionA).sqrMagnitude
            <= combinedRadius * combinedRadius;
    }

    /// <summary>
    /// Checks whether two axis-aligned boxes overlap.
    /// </summary>
    /// <param name="positionA">Center position of the first box.</param>
    /// <param name="halfSizeA">Half-size of the first box.</param>
    /// <param name="positionB">Center position of the second box.</param>
    /// <param name="halfSizeB">Half-size of the second box.</param>
    /// <returns>True when the two boxes overlap.</returns>
    public static bool BoxOverlap(
        Vector3 positionA,
        Vector3 halfSizeA,
        Vector3 positionB,
        Vector3 halfSizeB)
    {
        Vector3 dist = positionB - positionA;

        return
            Mathf.Abs(dist.x) <= halfSizeA.x + halfSizeB.x &&
            Mathf.Abs(dist.y) <= halfSizeA.y + halfSizeB.y &&
            Mathf.Abs(dist.z) <= halfSizeA.z + halfSizeB.z;
    }
}
