using UnityEngine;
using System;

[ExecuteAlways]
public class CustomBoxCollider : MonoBehaviour, CustomCollider
{
    [SerializeField] private bool isTrigger = false;
    [SerializeField] private bool isCollideable = true;
    [SerializeField] private Vector3 center = new Vector3(0, 0, 0);
    [SerializeField] private bool overrideCenter = false;
    [SerializeField] private Vector3 size = new Vector3(1, 1, 1);
    [SerializeField] private bool matchToObjectSize = true;
    [SerializeField] private Color gizmosColor = Color.red;
     
    public bool IsTrigger
    {
        get => isTrigger;
        set => isTrigger = value;
    }

    public bool IsCollideable
    {
        get => isCollideable;
        set => isCollideable = value;
    }

    public bool IsActive
    {
        get => this != null && isActiveAndEnabled;
        set => enabled = value;
    }

    public event Action<CustomCollider, Vector3> CollisionContact;
    public event Action<CustomCollider> TriggerEnter;
    public event Action<CustomCollider> TriggerStay;
    public event Action<CustomCollider> TriggerExit;
    
    private CollisionManager registeredManager;
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

    private void OnEnable()
    {
        RegisterWithManager();
    }
    private void Start()
    {
        RegisterWithManager();
    }

    private void Update()
    {
        SyncSize();
    }

    private void OnDisable()
    {
        UnregisterFromManager();
    }

    private void OnDestroy()
    {
        UnregisterFromManager();
    }

    public void NotifyCollision(CustomCollider other)
    {
        if (!isCollideable || !other.IsCollideable) return;

        ColliderBounds currentBounds = Bounds;
        ColliderBounds otherCurrentBounds = other.Bounds;

        Vector3 minMovement = currentBounds.min - PreviousBounds.min;
        Vector3 maxMovement = currentBounds.max - PreviousBounds.max;
        Vector3 otherMinMovement = otherCurrentBounds.min - other.PreviousBounds.min;
        Vector3 otherMaxMovement = otherCurrentBounds.max - other.PreviousBounds.max;
        
        Vector3 relativeMinMovement = minMovement - otherMaxMovement;
        Vector3 relativeMaxMovement = maxMovement - otherMinMovement;

        float contactTolerance = CollisionManager.ContactTolerance;

        bool wasOnLeft = PreviousBounds.max.x <= other.PreviousBounds.min.x + contactTolerance;
        bool wasAbove = PreviousBounds.min.y >= other.PreviousBounds.max.y - contactTolerance;
        bool wasInFront = PreviousBounds.max.z <= other.PreviousBounds.min.z + contactTolerance;

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

        Vector3 gapReduction = new Vector3(
            wasOnLeft ? relativeMaxMovement.x : -relativeMinMovement.x,
            wasAbove ? -relativeMinMovement.y : relativeMaxMovement.y,
            wasInFront ? relativeMaxMovement.z : -relativeMinMovement.z
        );
        
        float movementFractionAtContact = -1f;
        Vector3 contactAxes = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            float previousAxisGap = previousGap[axis];
            float axisGapReduction = gapReduction[axis];

        if (previousAxisGap < -contactTolerance || axisGapReduction <= 0f)
            continue;

        previousAxisGap = Mathf.Max(previousAxisGap, 0f);

        if (previousAxisGap > axisGapReduction) continue;

        float axisMovementFractionAtContact = previousAxisGap / axisGapReduction;

        if (axisMovementFractionAtContact < movementFractionAtContact) continue;

        if (axisMovementFractionAtContact > movementFractionAtContact)
        {
            movementFractionAtContact = axisMovementFractionAtContact;
            contactAxes = Vector3.zero;
        }

            contactAxes[axis] = 1f;
        }
                
        if (movementFractionAtContact < 0f) return;

        Vector3 minAtContact = PreviousBounds.min + minMovement * movementFractionAtContact;
        Vector3 maxAtContact = PreviousBounds.max + maxMovement * movementFractionAtContact;
        Vector3 otherMinAtContact = other.PreviousBounds.min + otherMinMovement * movementFractionAtContact;
        Vector3 otherMaxAtContact = other.PreviousBounds.max + otherMaxMovement * movementFractionAtContact;

        ApplyBounds(minAtContact, maxAtContact, contactAxes);
        other.ApplyBounds(otherMinAtContact, otherMaxAtContact, contactAxes);

        Vector3 contactDirection = new Vector3(
            wasOnLeft ? -contactAxes.x : contactAxes.x,
            wasAbove ? contactAxes.y : -contactAxes.y,
            wasInFront ? -contactAxes.z : contactAxes.z
        );

        if (registeredManager != null)
        {
            registeredManager.QueueCollisionContact(
                this, other, contactDirection);
        }
    }

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

    public void NotifyCollisionContact(CustomCollider other, Vector3 contactDirection)
    {
        CollisionContact?.Invoke(other, contactDirection);
    }

    public void NotifyTriggerEnter(CustomCollider other)
    {
        TriggerEnter?.Invoke(other);
    }

    public void NotifyTriggerStay(CustomCollider other)
    {
        TriggerStay?.Invoke(other);
    }

    public void NotifyTriggerExit(CustomCollider other)
    {
        TriggerExit?.Invoke(other);
    }

    private void RegisterWithManager()
    {
        if (!Application.IsPlaying(gameObject)) return;

        if (!isActiveAndEnabled || registeredManager != null || CollisionManager.Instance == null)
            return;

        PreviousBounds = Bounds;

        registeredManager = CollisionManager.Instance;
        registeredManager.Register(this);
    }

    private void UnregisterFromManager()
    {
        CollisionManager manager = registeredManager;
        registeredManager = null;

        if (manager != null)
            manager.Unregister(this);
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
