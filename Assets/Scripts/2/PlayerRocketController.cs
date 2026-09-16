using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRocketController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference fireAction;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Rocket Firing")]
    [SerializeField] private float fireRocketCooldown = 3f;
    [SerializeField] GameObject rocketPrefab;

    /*
     * Stores the player's current movement speed and direction.
     * This changes gradually toward targetSpeed instead of changing instantly.
     */
    private Vector3 currSpeed = Vector3.zero;

    private bool canFire = true;

    private void OnEnable()
    {
        moveAction.action.Enable();
        fireAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        fireAction.action.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        #region Movement

        /*
         * Read the player's 2D movement input.
         * X represents left/right, while Y represents forward/backward.
         */
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        /*
         * Cardinal-only movement:
         * compare the strength of the horizontal and vertical inputs.
         *
         * Mathf.Abs() is used because negative input should have the same
         * strength as positive input. For example, -1 and 1 are both full input.
         *
         * Whichever axis has the stronger input is kept.
         * The weaker axis is set to zero, preventing diagonal movement.
         *
         * If both axes are equally strong, Y is kept because the else branch runs.
         */
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            moveInput.y = 0f;
        }
        else
        {
            moveInput.x = 0f;
        }

        /*
         * Convert the 2D input into a 3D movement direction.
         *
         * X input moves along the object's right direction.
         * Y input moves along the object's forward direction.
         *
         * Because one input axis was removed above, movement can only point
         * forward, backward, left, or right.
         */
        Vector3 movement =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        /*
         * targetSpeed represents the velocity we WANT the player to eventually reach.
         *
         * movement gives the direction.
         * maxSpeed determines how fast the player should move in that direction.
         *
         */
        Vector3 targetSpeed = movement * maxSpeed;

        /*
         * sqrMagnitude tells us whether the movement vector has meaningful length.
         *
         * If movement is almost zero, the player is not giving movement input.
         * Using sqrMagnitude avoids calculating a square root just to check this.
         */
        bool isMoving = movement.sqrMagnitude > 0.001f;

        /*
         * Choose how quickly currSpeed is allowed to change.
         *
         * While input exists, use acceleration so the player speeds up.
         * Without input, use deceleration so the player slows down.
         */
        float currentRate = isMoving ? acceleration : deceleration;

        /*
         * Gradually change currSpeed toward targetSpeed.
         *
         * MoveTowards changes the velocity value toward the desired velocity.
         *
         * currentRate tells us how much the speed may change per second.
         * Time.deltaTime converts that into the amount allowed during this frame.
         *
         */
        currSpeed = Vector3.MoveTowards(
            currSpeed,
            targetSpeed,
            currentRate * Time.deltaTime);

        /*
         * Apply the current velocity to the player's position.
         *
         * Velocity is measured in distance per second.
         * Multiplying by deltaTime gives the distance travelled during this frame.
         *
         * position change = velocity × time
         */
        transform.position += currSpeed * Time.deltaTime;

        #endregion

        #region Rocket Firing

        if (fireAction.action.WasPerformedThisFrame())
        {
            FireRocket();
        }

        #endregion
    }

    private void FireRocket()
    {
        if (!canFire) return;

        // var rocket = Instantiate(rocketPrefab, transform.position, Quaternion.LookRotation(transform.forward));

        StartCoroutine(StartFireCooldown());
    }

    private IEnumerator StartFireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRocketCooldown);
        canFire = true;
    }
}