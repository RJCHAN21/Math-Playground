using UnityEngine;
using UnityEngine.SceneManagement;

public class NullZone : MonoBehaviour
{
    [Header("NullZone Settings")]
    [SerializeField] private Transform nullZone;

    [Tooltip("Distance at which the player triggers the NullZone warning.")]
    [SerializeField] private float proximityThreshold;

    [Tooltip("Maximum distance from the NullZone where warning behavior begins.")]
    [SerializeField] private float maximumProximityThreshold = 5f;
    
    [SerializeField] private float shakeStrength = 0.1f;

    [Tooltip("The NullZone will use this material to signal a visual warning to the player.")]
    [SerializeField] private Material warningMaterial;

    [Header("Player")]
    [SerializeField] private Transform player;

    private bool isWarning = false;
    private Vector3 origPos;
    private Renderer objRend;
    private Color origColor;

    void Awake()
    {
        if (nullZone == null)
            nullZone = gameObject.transform;

        origPos = nullZone.transform.localPosition;
        objRend = GetComponent<Renderer>();
        origColor = objRend.material.color;
    }

    void Update()
    {
        // Convert the NullZone's stored local position into world-space coordinates
        // so the distance check remains correct even while the NullZone visually shakes.
        Vector3 worldOrigPos = nullZone.parent != null
            ? nullZone.parent.TransformPoint(origPos)
            : origPos;

        // Measure the player's distance from the center using the NullZone's stable position.
        var dir = player.position - worldOrigPos;
        float sqrDistance = dir.sqrMagnitude;

        var randomOffset = Random.insideUnitSphere * shakeStrength;

        // Use the NullZone's bounds to determine whether the player is inside it.
        Bounds zoneBounds = objRend.bounds;
        zoneBounds.center = worldOrigPos;

        bool isInsideZoneBounds = zoneBounds.Contains(player.position);

        // Measure how close the player is to the NullZone boundary while outside it.
        Vector3 closestPoint = zoneBounds.ClosestPoint(player.position);
        float distanceToZone = Vector3.Distance(player.position, closestPoint);

        // Restart only after the player is inside the NullZone and reaches the maximum depth.
        if (isInsideZoneBounds &&
            sqrDistance >= maximumProximityThreshold * maximumProximityThreshold)
        {
            Debug.Log("Scene restarted due to Player exceeding the NullZone's maximum boundary.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        // Warn when the player is inside the NullZone or close enough to it from the outside.
        else if (isInsideZoneBounds || distanceToZone <= proximityThreshold)
        {
            nullZone.localPosition = origPos + randomOffset;

            if (!isWarning)
            {
                objRend.material = warningMaterial;
                isWarning = true;
            }
        }
        // Restore the NullZone when the player is safely away from it.
        else
        {
            objRend.material.color = origColor;
            nullZone.localPosition = origPos;
            isWarning = false;
        }
    }
}