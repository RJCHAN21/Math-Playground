using UnityEngine;

public class ConeGizmos : MonoBehaviour
{

    [Header("Cone-Shaped Gizmos Settings")]
    [SerializeField] private float length = 2f;
    [SerializeField] private int segments = 16;
    [SerializeField] private float radius = 2f;
    private void OnDrawGizmos()
    {
        // Draws a cone facing forwards of the object.
        var coneTip = transform.position;
        var coneBaseCenter = transform.position + transform.forward * length;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f/ segments;
            float nextAngle = angle + (Mathf.PI * 2f/ segments);

            Vector3 offset =
                transform.right * Mathf.Cos(angle) * radius +
                transform.up * Mathf.Sin(angle) * radius;

            Vector3 circlePt = coneBaseCenter + offset;
            // Gizmos.DrawSphere(circlePt, 0.05f);

            Vector3 nextOffset =
                transform.right * Mathf.Cos(nextAngle) * radius +
                transform.up * Mathf.Sin(nextAngle) * radius;

            Vector3 nextCirclePt = coneBaseCenter + nextOffset;
            Gizmos.DrawLine(circlePt, nextCirclePt);
            Gizmos.DrawLine(circlePt, coneTip);
        }
    }
}
