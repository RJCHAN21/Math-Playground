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

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        // Allow movement on only one axis at a time.
        // If both axes are equal, vertical movement takes priority.
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

        bool isMoving = movement.sqrMagnitude > 0.001f;
        float currentRate = isMoving ? acceleration : deceleration;
        
        currSpeed = Vector3.MoveTowards(currSpeed, targetSpeed, currentRate * Time.deltaTime);

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
