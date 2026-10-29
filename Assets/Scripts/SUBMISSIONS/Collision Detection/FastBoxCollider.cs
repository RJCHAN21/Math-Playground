using UnityEngine;

public class FastBoxCollider : MonoBehaviour, FastCollider
{
    [SerializeField] private Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] private bool overrideCenter = false;
    [SerializeField] private Vector3 size = new Vector3(1, 1, 1);
    [SerializeField] private bool matchToObjectSize = true;
    [SerializeField] private Color gizmosColor = Color.red;
     
    private Vector3 lastMatchedSize;
    private bool sizeMatchInitialized;

    public ColliderBounds Bounds
    {
        get
        {
            SyncSize();
            Vector3 selectedCenter = overrideCenter ? center : transform.position;
            return new ColliderBounds(size, selectedCenter);
        }
    }

    public ColliderBounds PreviousBounds { get; set; }

    private void Start()
    {
        if (CollisionManager.Instance != null)
        {
            CollisionManager.Instance.Register(this);
            PreviousBounds = Bounds;
        }
            
    }

    private void Update()
    {
        SyncSize();
    }

    private void OnDestroy()
    {
        if (CollisionManager.Instance != null)
            CollisionManager.Instance.Unregister(this);    
    }

    public void NotifyCollision(FastCollider other)
    {
        Debug.Log($"{gameObject.name} collided with {((Component)other).gameObject.name}");

        // NOTE: We're checking both min and max to also stop a growing object from physically colliding with another object.

        Vector3 minMovement = Bounds.min - PreviousBounds.min;
        Vector3 maxMovement = Bounds.max - PreviousBounds.max;
        Vector3 otherMinMovement = other.Bounds.min - other.PreviousBounds.min;
        Vector3 otherMaxMovement = other.Bounds.max - other.PreviousBounds.max;

        // How differently did the opposing boundaries move?
        Vector3 relativeMinMovement = minMovement - otherMaxMovement;
        Vector3 relativeMaxMovement = maxMovement - otherMinMovement;

        bool wasOnLeft = PreviousBounds.max.x <= other.PreviousBounds.min.x;
        bool wasAbove = PreviousBounds.min.y >= other.PreviousBounds.max.y;
        bool wasInFront = PreviousBounds.max.z <= other.PreviousBounds.min.z;

        // How much space was previously between the selected opposing faces?
        Vector3 previousGap = new Vector3(
            wasOnLeft
                ? other.PreviousBounds.min.x - PreviousBounds.max.x
                : PreviousBounds.min.x - other.PreviousBounds.max.x,

            wasAbove
                ? PreviousBounds.min.y - other.PreviousBounds.max.y
                : other.PreviousBounds.min.y - PreviousBounds.max.y,

            wasInFront
                ? other.PreviousBounds.min.z - PreviousBounds.max.z
                : PreviousBounds.min.z - other.PreviousBounds.max.z
        );

        // How much did that gap shrink during the attempted change?
        Vector3 gapReduction = new Vector3(
            wasOnLeft ? relativeMaxMovement.x : -relativeMinMovement.x,
            wasAbove ? -relativeMinMovement.y : relativeMaxMovement.y,
            wasInFront ? relativeMaxMovement.z : -relativeMinMovement.z
        );
        
        // -1 means no valid contact fraction has been found yet.
        float movementFractionAtContact = -1f;
        Vector3 contactAxes = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            float previousAxisGap = previousGap[axis];
            float axisGapReduction = gapReduction[axis];

            bool reachedContact =
                previousAxisGap >= 0f // Were the faces previously separated or touching?
                && axisGapReduction > 0f // Did their gap shrink?
                && previousAxisGap <= axisGapReduction; // Was that reduction enough to reach contact?
            
            if (!reachedContact) continue;

            // How much of the attempted movement was allowed before this axis’s faces touched?
            float axisMovementFractionAtContact = previousAxisGap / axisGapReduction;

            if (axisMovementFractionAtContact < movementFractionAtContact) continue;

            // NOTE: We select the largest valid contact fraction for contacts occuring in multiple axes.
            if (axisMovementFractionAtContact > movementFractionAtContact)
            {
                movementFractionAtContact = axisMovementFractionAtContact;
                contactAxes = Vector3.zero;
            }

            contactAxes[axis] = 1f;
        }
                
        if (movementFractionAtContact < 0f) return;

        // This block says to move it only through the allowed fraction.

        Vector3 minAtContact = PreviousBounds.min + minMovement * movementFractionAtContact;
        Vector3 maxAtContact = PreviousBounds.max + maxMovement * movementFractionAtContact;
        Vector3 otherMinAtContact = other.PreviousBounds.min + otherMinMovement * movementFractionAtContact;
        Vector3 otherMaxAtContact = other.PreviousBounds.max + otherMaxMovement * movementFractionAtContact;

        ApplyBounds(minAtContact, maxAtContact, contactAxes);
        other.ApplyBounds(otherMinAtContact, otherMaxAtContact, contactAxes);
    }

    // contactAxes serves as a marker on which axes to constrain.
    public void ApplyBounds(Vector3 min, Vector3 max, Vector3 contactAxes)
    {
        SyncSize();

        Vector3 centerAtContact = (min + max) / 2f;
        Vector3 sizeAtContact = max - min;
        Vector3 selectedCenter = overrideCenter ? center : transform.position;

        for (int axis = 0; axis < 3; axis++)
        {
            if (contactAxes[axis] == 0f) continue;

            selectedCenter[axis] = centerAtContact[axis];
            size[axis] = sizeAtContact[axis];
        }

        if (overrideCenter)
            center = selectedCenter;
        else
            transform.position = selectedCenter;

        SyncSize();
    }

    private void SyncSize()
    {
        if (!matchToObjectSize)
        {
            sizeMatchInitialized = false;
            return;
        }
        if (!sizeMatchInitialized)
        {
            size = transform.localScale;
            sizeMatchInitialized = true;
        }
        else if (!size.Equals(lastMatchedSize)) {
            transform.localScale = size;
        }
        else
        {
            size = transform.localScale;
        }

        lastMatchedSize = size;
    }

    private void OnDrawGizmos()
    {
        Vector3 selectedCenter = overrideCenter ? center : transform.position;
        CustomGizmos.DrawWireCube(selectedCenter, size, gizmosColor);
    }
}
