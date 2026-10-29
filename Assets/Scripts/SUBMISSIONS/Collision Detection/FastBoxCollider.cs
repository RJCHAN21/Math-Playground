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

        // The difference between how much their opposing faces have moved
        Vector3 relativeMinMovement = minMovement - otherMaxMovement;
        Vector3 relativeMaxMovement = maxMovement - otherMinMovement;

        bool wasOnLeft = PreviousBounds.max.x <= other.PreviousBounds.min.x;

        float previousGap = wasOnLeft
            ? other.PreviousBounds.min.x - PreviousBounds.max.x
            : PreviousBounds.min.x - other.PreviousBounds.max.x;

        // This is just relativeClosingMovement but I renamed it to gapReduction for clarity.
        // We use our corresponding side facing the other collider compared to their side facing us.
        float gapReduction = wasOnLeft 
            ? relativeMaxMovement.x
            : -relativeMinMovement.x;
        
        if (previousGap >= 0f && gapReduction > 0f && previousGap <= gapReduction)
        {
            // This whole block basically says "How much of the attempted movement was allowed before contact?"

            float movementFractionAtContact = previousGap / gapReduction;
            float minXAtContact = PreviousBounds.min.x + minMovement.x * movementFractionAtContact;
            float maxXAtContact = PreviousBounds.max.x + maxMovement.x * movementFractionAtContact;
            float otherMinXAtContact = other.PreviousBounds.min.x + otherMinMovement.x * movementFractionAtContact;
            float otherMaxAtContact = other.PreviousBounds.max.x + otherMaxMovement.x * movementFractionAtContact;

            ApplyXBounds(minXAtContact, maxXAtContact);
            other.ApplyXBounds(otherMinXAtContact, otherMaxAtContact);
        }
    }

    public void ApplyXBounds(float minX, float maxX)
    {
        float centerX = (minX + maxX) / 2f;
        float width = maxX - minX;

        size.x = width;

        if (overrideCenter)
        {
            center.x = centerX;
        }
        else
        {
            Vector3 position = transform.position;
            position.x = centerX;
            transform.position = position;
        }

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
