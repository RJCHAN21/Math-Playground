using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }
    public const float ContactTolerance = 0.001f;
    private List<CustomCollider> colliders = new();
    private HashSet<CustomCollider> registeredColliders = new();
    private HashSet<(CustomCollider receiver, CustomCollider other)> triggerContacts = new();
    private HashSet<(CustomCollider receiver, CustomCollider other)> nextTriggerContacts = new();
    private HashSet<(CustomCollider receiver, CustomCollider other)> enteredTriggerContacts = new();
    private List<(CustomCollider receiver, CustomCollider other)> contactsToRemove = new();
    private Queue<(CustomCollider receiver, CustomCollider other, TriggerPhase phase)> triggerNotifications = new();
    private Queue<(CustomCollider receiver, CustomCollider other, Vector3 direction)> collisionNotifications = new();

    private enum TriggerPhase
    {
        Enter,
        Stay,
        Exit
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        nextTriggerContacts.Clear();

        for (int i = 0; i < colliders.Count; i++)
        {
            CustomCollider collider = colliders[i];
            ColliderBounds currentBounds = collider.Bounds;

            for (int j = i + 1; j < colliders.Count; j++)
            {
                CustomCollider other = colliders[j];
                ColliderBounds otherCurrentBounds = other.Bounds;

                if (!IsColliding(currentBounds, otherCurrentBounds))
                    continue;
                
                if (collider.IsTrigger)
                    nextTriggerContacts.Add((collider, other));
                
                if (other.IsTrigger)
                    nextTriggerContacts.Add((other, collider));
                
                if (collider.IsCollideable && other.IsCollideable)
                {
                    collider.NotifyCollision(other);
                    currentBounds = collider.Bounds;
                }
            }
        }

        foreach (var contact in nextTriggerContacts)
        {
            TriggerPhase phase = triggerContacts.Contains(contact)
                ? TriggerPhase.Stay
                : TriggerPhase.Enter;
            
            triggerNotifications.Enqueue((contact.receiver, contact.other, phase));
        }

        foreach (var contact in triggerContacts)
        {
            if (!nextTriggerContacts.Contains(contact))
            {
                triggerNotifications.Enqueue((contact.receiver, contact.other, TriggerPhase.Exit));
            }
        }

        var previousTriggerContacts = triggerContacts;
        triggerContacts = nextTriggerContacts;
        nextTriggerContacts = previousTriggerContacts;


        foreach (CustomCollider collider in colliders)
        {
            collider.PreviousBounds = collider.Bounds;
        }

        DispatchCollisionNotifications();
        DispatchTriggerNotifications();
    }

    public void Register(CustomCollider collider)
    {
        if (!registeredColliders.Add(collider)) return;
        colliders.Add(collider);
    }

    public void Unregister(CustomCollider collider)
    {
        if (!registeredColliders.Remove(collider)) return;
        colliders.Remove(collider);
        contactsToRemove.Clear();

        foreach (var contact in triggerContacts)
        {
            if (contact.receiver == collider || contact.other == collider)
            {
                contactsToRemove.Add(contact);
            }
        }

        foreach (var contact in contactsToRemove)
        {
            triggerContacts.Remove(contact);
            triggerNotifications.Enqueue((contact.receiver, contact.other, TriggerPhase.Exit));
        }

        contactsToRemove.Clear();
    }

    public void QueueCollisionContact(CustomCollider first, CustomCollider second, Vector3 contactDirection)
    {
        collisionNotifications.Enqueue((first, second, contactDirection));
        collisionNotifications.Enqueue((second, first, -contactDirection));
    }

    private bool IsRegisteredAndActive(CustomCollider collider)
    {
        return collider != null && registeredColliders.Contains(collider) && collider.IsActive;
    }

    private void DispatchCollisionNotifications()
    {
        int notifCount = collisionNotifications.Count;

        for (int i = 0; i < notifCount; i++)
        {
            var notif = collisionNotifications.Dequeue();

            if (!IsRegisteredAndActive(notif.receiver)
                || !IsRegisteredAndActive(notif.other)
                || !notif.receiver.IsCollideable
                || !notif.other.IsCollideable)
            {
                continue;
            }

            Vector3 contactDirection = GetValidContactDirection(
                notif.receiver.Bounds,
                notif.other.Bounds,
                notif.direction);

            if (contactDirection == Vector3.zero)
                continue;

            notif.receiver.NotifyCollisionContact(
                notif.other, contactDirection);
        }
    }

    private Vector3 GetValidContactDirection(ColliderBounds receiverBounds, ColliderBounds otherBounds, Vector3 contactDirection)
    {
        Vector3 validDirection = Vector3.zero;

        for (int axis = 0; axis < 3; axis++)
        {
            if (contactDirection[axis] == 0f)
                continue;

            float gap = contactDirection[axis] > 0f
                ? receiverBounds.min[axis] - otherBounds.max[axis]
                : otherBounds.min[axis] - receiverBounds.max[axis];

            if (Mathf.Abs(gap) > ContactTolerance)
                continue;

            bool overlapsOtherAxes = true;

            for (int otherAxis = 0; otherAxis < 3; otherAxis++)
            {
                if (otherAxis == axis)
                    continue;

                if (receiverBounds.max[otherAxis] < otherBounds.min[otherAxis] - ContactTolerance
                    || receiverBounds.min[otherAxis] > otherBounds.max[otherAxis] + ContactTolerance)
                {
                    overlapsOtherAxes = false;
                    break;
                }
            }

            if (overlapsOtherAxes)
                validDirection[axis] = contactDirection[axis];
        }

        return validDirection;
    }

    private void DispatchTriggerNotifications()
    {
        int notifCount = triggerNotifications.Count;

        for (int i = 0; i < notifCount; i++)
        {
            var notif = triggerNotifications.Dequeue();

            (CustomCollider receiver, CustomCollider other) contact = (notif.receiver, notif.other);

            if (notif.phase == TriggerPhase.Exit)
            {
                if (!enteredTriggerContacts.Remove(contact))
                    continue;

                if (IsRegisteredAndActive(notif.receiver))
                    notif.receiver.NotifyTriggerExit(notif.other);

                continue;
            }

            if (!IsRegisteredAndActive(notif.receiver)
                || !IsRegisteredAndActive(notif.other)
                || !notif.receiver.IsTrigger
                || !triggerContacts.Contains(contact))
            {
                continue;
            }

            if (enteredTriggerContacts.Add(contact))
            {
                notif.receiver.NotifyTriggerEnter(notif.other);
            }
            else if (notif.phase == TriggerPhase.Stay)
            {
                notif.receiver.NotifyTriggerStay(notif.other);
            }
        }
    }

    public bool IsColliding(ColliderBounds a, ColliderBounds b)
    {
        return a.min.x <= b.max.x
            && b.min.x <= a.max.x
            && a.min.y <= b.max.y
            && b.min.y <= a.max.y
            && a.min.z <= b.max.z
            && b.min.z <= a.max.z;
    }
}
