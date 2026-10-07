using UnityEngine;

public class HomingMissile : MonoBehaviour
{
#region Unity Inspector Fields
    [SerializeField] private float hitRadius = 1f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 0.65f;
#endregion

#region Runtime Properties
    private Transform target;
    private SimplePlayer playerController;
#endregion

#region Unity Life Cycle
    private void Update()
    {
        if (playerController == null || target == null)
        {
            Debug.LogError("The missile has no target.");
            return;
        }

        HomeInTarget(target);

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= hitRadius + playerController.hitRadius)
        {
            playerController.onHit.Invoke();
            Destroy(gameObject);
        }

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
#endregion

#region Targeting Logic
    public void SetTarget(SimplePlayer player)
    {
        playerController = player;
        target = player.transform;
    }

    private void HomeInTarget(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;

        if (dir.sqrMagnitude == 0f) return;

        dir.Normalize();

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
        transform.position += transform.up * speed * Time.deltaTime;
    }
#endregion
}
