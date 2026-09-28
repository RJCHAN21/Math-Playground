using UnityEngine;

/// <summary>
/// Visualizes a fixed world-space detection cone.
/// </summary>
public class ConeGizmoVisualizer : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform target;
    [SerializeField] private float range = 5f;
    [SerializeField] private float coneAngle = 60f;

    [Header("Visualization")]
    [SerializeField] private int segments = 24;

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        /*
         * Fixed world-space direction.
         * This prevents the visualization from rotating
         * together with this GameObject.
         */
        Vector3 forward = Vector3.forward;

        Vector3 dirToTarget = target != null
            ? target.position - origin
            : Vector3.zero;

        bool detected =
            target != null &&
            dirToTarget.magnitude <= range &&
            Vector3.Angle(forward, dirToTarget) <= coneAngle / 2f;

        Gizmos.color = detected ? Color.red : Color.cyan;

        float halfAngle = coneAngle / 2f;
        Quaternion coneRotation = Quaternion.LookRotation(forward);

        Vector3 previousPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float aroundAngle = i * 360f / segments;

            /*
             * Creates a direction sitting exactly on the
             * outer angular boundary of the cone.
             */
            Vector3 localDirection = Quaternion.Euler(
                0f,
                halfAngle,
                aroundAngle
            ) * Vector3.forward;

            Vector3 direction = coneRotation * localDirection;
            Vector3 point = origin + direction * range;

            if (i > 0)
                Gizmos.DrawLine(previousPoint, point);

            /*
             * Draw several of the lines from the origin
             * so the 3D cone shape is easy to see.
             */
            if (i % 4 == 0)
                Gizmos.DrawLine(origin, point);

            previousPoint = point;
        }

        // Shows the center and maximum range.
        Gizmos.DrawLine(origin, origin + forward * range);
    }
}