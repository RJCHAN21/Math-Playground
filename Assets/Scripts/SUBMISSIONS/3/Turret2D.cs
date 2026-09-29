using UnityEngine;

public class Turret2D : MonoBehaviour
{
#region Inspector Fields

    [Header("Target"), Tooltip("Object for the turret to target and shoot.")]
    [SerializeField] private Transform target;

    [Header("Turret Config")]
    [Tooltip("How far the turret can see.")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float turningSpeed = 90f;
    [SerializeField] private TurretFiringType firingType;
    

    [Tooltip("The angle (in degrees) that this turret can see in a field-of-view arc.")]
    [SerializeField] private float detectionAngle = 60f;

    [Header("Pooling")]
    [SerializeField] private PooledGun pooledGun;

    [Header("Debug")]
    [SerializeField] private LineRenderer lineRenderer;
#endregion

    private float? appliedDetectionAngle;
    private TurretFiringType? appliedFiringType;
    private const float SniperLineWidth = 0.2f;
    private bool canFire = true;

#region Unity Lifecycle
    private void Start()
    {
        if (target != null)
            pooledGun.SetHitTarget(target.GetComponent<IHittable2D>());
    }

    private void Update()
    {
        if (!canFire) return;
        
        if (appliedFiringType != firingType ||
            appliedDetectionAngle != detectionAngle)
        {
            switch (firingType)
            {
                case TurretFiringType.Flamethrower:
                    range = 10f;
                    pooledGun.SetFiringMode(new Flamethrower(detectionAngle));
                    break;

                case TurretFiringType.Sniper:
                    range = 50f;
                    pooledGun.SetFiringMode(new Sniper());
                    break;

                case TurretFiringType.Shotgun:
                    range = 10f;
                    pooledGun.SetFiringMode(new Shotgun());
                    break;
            }          
            appliedFiringType = firingType;
            appliedDetectionAngle = detectionAngle;
        }

        MapVisualToRange();

        if (target == null) return;

        bool targetDetected = firingType == TurretFiringType.Sniper 
            ? SightDetector2D.IsInLine(transform, target, range, SniperLineWidth * 0.5f) 
            : SightDetector2D.IsInCone(transform, target, range, detectionAngle);
                

        if (targetDetected)
        {
            LookAtTarget(target);
            if (canFire && IsFacingTarget(target))
            {
                pooledGun.ShootWithFiringMode();
            }
        }
    }
#endregion

#region Attack Behavior
    private void LookAtTarget(Transform target)
    {
        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0f, 0f, angle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            desiredRotation,
            turningSpeed * Time.deltaTime
        );
    }

    private bool IsFacingTarget(Transform target)
    {
        Vector2 forward = transform.right;
        Vector2 toTarget = ((Vector2)target.position
            - (Vector2)transform.position).normalized;

        float dot = Vector2.Dot(forward, toTarget);
        bool facingTarget = dot >= Mathf.Cos(2f * Mathf.Deg2Rad);

        return facingTarget;
    }

    public void DeactivateTurret()
    {
        canFire = false;
    }
#endregion

#region LineRenderer
    /// <summary>
    /// Adapts the LineRenderer so that it closely matches the Cone line of sight shape.
    /// Note that this is only creates a closer approximate, so it may not perfectly fit the real cone area drawn by code.
    /// </summary>
    private void MapVisualToRange()
    {
        float scale = lineRenderer.transform.lossyScale.x;

        if (firingType == TurretFiringType.Sniper)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, new Vector3(range / scale, 0f, 0f));
            lineRenderer.widthMultiplier = SniperLineWidth / scale;
            lineRenderer.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            return;
        }
        int points = 33;
        lineRenderer.positionCount = points;

        float halfAngleRad = detectionAngle / 2f * Mathf.Deg2Rad;
        float sideSlope = Mathf.Tan(halfAngleRad);
        
        Keyframe[] widths = new Keyframe[points];
        
        // This part's kinda complicated and took me a long while to make so I'm leaving future me some notes.

        // Creates points along the LineRenderer and calculates the width at each point so the connected line forms the cone shape.
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float distance = range * t;
            // halfWidth only measures from the center line to one side.
            float halfWidth = Mathf.Min(
            // Mathf.Min uses whichever boundary is more restrictive at this point.
            // This asks "At this distance forward, how wide is the cone allowed to be based on its angle?"
                distance * sideSlope,
            // Then asks "At this distance forward, how wide can we still be without leaving the circular maximum range?"
                Mathf.Sqrt(range * range - distance * distance)
            );

        
            lineRenderer.SetPosition(i, new Vector3(distance / scale, 0f, 0f));
            widths[i] = new Keyframe(t, 2f * halfWidth / scale);
        }

        lineRenderer.widthMultiplier = 1f;
        lineRenderer.widthCurve = new AnimationCurve(widths);
    }
#endregion

}
