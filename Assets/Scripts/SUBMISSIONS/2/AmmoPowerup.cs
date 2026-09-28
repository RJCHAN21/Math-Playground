using UnityEngine;

public class AmmoPowerup : MonoBehaviour
{
    #region Fields

    [Header("Powerup Settings")]
    [Tooltip("Amount of ammo to reward to the player upon interaction.")]
    [SerializeField] private int rewardAmount = 1;

    [Tooltip("Time it takes for the power-up to despawn.")]
    [SerializeField] private float lifetime = 10f;

    [Header("Collision Settings")]
    [SerializeField] private float collisionRadius = 1f;
    [SerializeField] private float targetRadius = 0.5f;

    private Transform target;
    private bool wasColliding;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        if (PlayerRocketController.Instance == null)
        {
            Debug.LogError("Could not find player!");
            enabled = false;
            return;
        }

        target = PlayerRocketController.Instance.transform;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        bool isColliding = ColliderDetector.SphereOverlap(
            transform.position,
            collisionRadius,
            target.position,
            targetRadius
        );

        if (isColliding && !wasColliding)
        {
            OnObjectEnter();
        }

        wasColliding = isColliding;
    }

    #endregion

    #region Powerup Behavior

    private void OnObjectEnter()
    {
        bool rocketsAdded =
            PlayerRocketController.Instance.AddRockets(rewardAmount);

        if (rocketsAdded)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}