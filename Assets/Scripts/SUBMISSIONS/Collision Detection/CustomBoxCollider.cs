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
        RegisterWithManager();
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

        Vector3 previousMinFaceGap = PreviousBounds.min - other.PreviousBounds.max;
        Vector3 previousMaxFaceGap = other.PreviousBounds.min - PreviousBounds.max;

        Vector3 minFaceGapReduction = -relativeMinMovement;
        Vector3 maxFaceGapReduction = relativeMaxMovement;

        Vector3 currentMinFaceGap = previousMinFaceGap - minFaceGapReduction;
        Vector3 currentMaxFaceGap = previousMaxFaceGap - maxFaceGapReduction;

        float contactTolerance = CollisionManager.ContactTolerance;
        float movementFractionAtContact = -1f;
        float exitFraction = 1f;
        Vector3 contactDirection = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            for (int side = 0; side < 2; side++)
            {
                bool checkingMinFace = side == 0;

                float previousAxisGap = checkingMinFace
                    ? previousMinFaceGap[axis]
                    : previousMaxFaceGap[axis];

                float currentAxisGap = checkingMinFace
                    ? currentMinFaceGap[axis]
                    : currentMaxFaceGap[axis];

                float axisGapReduction = checkingMinFace
                    ? minFaceGapReduction[axis]
                    : maxFaceGapReduction[axis];

                if (previousAxisGap > 0f && currentAxisGap > 0f) return;

                if (currentAxisGap > 0f)
                {
                    float axisMovementFractionAtExit = previousAxisGap / axisGapReduction;

                    exitFraction = Mathf.Min(exitFraction, axisMovementFractionAtExit);
                }

                if (previousAxisGap < -contactTolerance || axisGapReduction <= 0f)
                    continue;

                previousAxisGap = Mathf.Max(previousAxisGap, 0f);

                float axisMovementFractionAtContact = previousAxisGap / axisGapReduction;

                if (axisMovementFractionAtContact < movementFractionAtContact)
                    continue;

                if (axisMovementFractionAtContact > movementFractionAtContact)
                {
                    movementFractionAtContact = axisMovementFractionAtContact;
                    contactDirection = Vector3.zero;
                }

                contactDirection[axis] = checkingMinFace ? 1f : -1f;
            }
        }

        if (movementFractionAtContact > exitFraction)
            return;

        if (movementFractionAtContact < 0f)
        {
            ResolveExistingOverlap(other, currentBounds, otherCurrentBounds);
            return;
        }

        Vector3 contactAxes = new Vector3(
            Mathf.Abs(contactDirection.x),
            Mathf.Abs(contactDirection.y),
            Mathf.Abs(contactDirection.z));

        Vector3 minAtContact = PreviousBounds.min + minMovement * movementFractionAtContact;
        Vector3 maxAtContact = PreviousBounds.max + maxMovement * movementFractionAtContact;
        Vector3 otherMinAtContact = other.PreviousBounds.min + otherMinMovement * movementFractionAtContact;
        Vector3 otherMaxAtContact = other.PreviousBounds.max + otherMaxMovement * movementFractionAtContact;

        ApplyBounds(minAtContact, maxAtContact, contactAxes);
        other.ApplyBounds(otherMinAtContact, otherMaxAtContact, contactAxes);

        if (registeredManager != null)
        {
            registeredManager.QueueCollisionContact(this, other, contactDirection);
        }
    }

    private void ResolveExistingOverlap(CustomCollider other, ColliderBounds currentBounds, ColliderBounds otherCurrentBounds)
    {
        Vector3 minFaceOverlap = otherCurrentBounds.max - currentBounds.min;
        Vector3 maxFaceOverlap = currentBounds.max - otherCurrentBounds.min;

        float smallestOverlap = float.PositiveInfinity;
        Vector3 contactDirection = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            float minAxisOverlap = minFaceOverlap[axis];
            float maxAxisOverlap = maxFaceOverlap[axis];

            if (minAxisOverlap <= 0f || maxAxisOverlap <= 0f)
                return;

            bool useMinFace = minAxisOverlap <= maxAxisOverlap;

            float axisOverlap = useMinFace
                ? minAxisOverlap
                : maxAxisOverlap;

            if (axisOverlap >= smallestOverlap)
                continue;

            smallestOverlap = axisOverlap;
            contactDirection = Vector3.zero;
            contactDirection[axis] = useMinFace ? 1f : -1f;
        }

        Vector3 contactAxes = new Vector3(
            Mathf.Abs(contactDirection.x),
            Mathf.Abs(contactDirection.y),
            Mathf.Abs(contactDirection.z));

        float halfOverlap = smallestOverlap / 2f;
        Vector3 movement = contactDirection * halfOverlap;

        Vector3 correctedMin = currentBounds.min + movement;
        Vector3 correctedMax = currentBounds.max + movement;
        Vector3 otherCorrectedMin = otherCurrentBounds.min - movement;
        Vector3 otherCorrectedMax = otherCurrentBounds.max - movement;

        ApplyBounds(correctedMin, correctedMax, contactAxes);
        other.ApplyBounds(otherCorrectedMin, otherCorrectedMax, contactAxes);

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
            size[axis] = size[axis] < 0f ? -sizeAtContact[axis] : sizeAtContact[axis];
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
