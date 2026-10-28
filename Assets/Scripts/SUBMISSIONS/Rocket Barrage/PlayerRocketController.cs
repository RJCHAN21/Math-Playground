using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRocketController : MonoBehaviour
{
    #region Fields

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Rocket Firing")]
    [SerializeField] private int startingRocketCount = 8;
    [SerializeField] private int maxRocketCount = 8;
    [SerializeField] private float fireRocketCooldown = 3f;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float spawnRadius = 2f;

    private Vector3 currSpeed = Vector3.zero;
    private float nextFireTime;
    private int currRocketCount;

    #endregion

    #region Properties

    public static PlayerRocketController Instance { get; private set; }

    public int CurrentRocketCount => currRocketCount;
    public int MaxRocketCount => maxRocketCount;

    public event Action<int> RocketCountChanged;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        Instance = this;
        currRocketCount = startingRocketCount;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        /*
         * Restrict movement to cardinal directions by keeping only
         * the strongest input axis. Y takes priority when both are equal.
         */
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            moveInput.y = 0f;
        }
        else
        {
            moveInput.x = 0f;
        }

        Vector3 movement =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        Vector3 targetSpeed = movement * maxSpeed;

        /*
         * sqrMagnitude should be sufficient as we only need to check
         * whether movement input exists, not calculate its actual magnitude.
         */
        bool isMoving = movement.sqrMagnitude > 0.001f;

        float currentRate = isMoving ? acceleration : deceleration;

        currSpeed = Vector3.MoveTowards(
            currSpeed,
            targetSpeed,
            currentRate * Time.deltaTime);

        transform.position += currSpeed * Time.deltaTime;
            
        FireRocket();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion

    #region Rocket Firing

    /// <summary>
    /// Adds rockets to the player's available rocket count without
    /// exceeding the maximum capacity.
    /// </summary>
    /// <param name="amount">Number of rockets to add.</param>
    /// <returns>True if at least one rocket could be added.</returns>
    public bool AddRockets(int amount)
    {
        if (currRocketCount >= MaxRocketCount)
            return false;
        
        currRocketCount = Mathf.Min(
            currRocketCount + amount,
            MaxRocketCount);

        RocketCountChanged?.Invoke(currRocketCount);
            
        return true;
    }

    private void FireRocket()
    {
        if (Time.time < nextFireTime || currRocketCount <= 0)
            return;

        nextFireTime = Time.time + fireRocketCooldown;

        int rocketsToFire = currRocketCount;

        SpawnRockets(rocketsToFire);

        currRocketCount -= rocketsToFire;

        RocketCountChanged?.Invoke(currRocketCount);
    }

    /// <summary>
    /// Spawns rockets evenly around the object in a 360-degree radial pattern.
    /// Each rocket is positioned at the spawn radius and rotated to face outward.
    /// </summary>
    /// <param name="rocketCount">The number of rockets to spawn.</param>
    private void SpawnRockets(int rocketCount)
    {
        float angleStep = 360f / rocketCount;
        float angle = 0f;

        for (int i = 0; i <= rocketCount - 1; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
            Vector3 spawnPos = transform.position + dir * spawnRadius;

            Instantiate(rocketPrefab, spawnPos, Quaternion.LookRotation(dir));

            angle += angleStep;
        }
    }

    #endregion
}