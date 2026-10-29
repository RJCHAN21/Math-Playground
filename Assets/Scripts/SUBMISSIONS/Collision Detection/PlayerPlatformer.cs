using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CustomBoxCollider))]
[DefaultExecutionOrder(-100)]
public class PlayerPlatformer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 8f;
    [SerializeField] private float gravity = 9.81f;

    private float horizontalInput;
    private float verticalSpeed;
    private bool jumpRequested;
    private bool isGrounded;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();

        CustomBoxCollider collider = GetComponent<CustomBoxCollider>();
        collider.CollisionContact += HandleContact;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();

        CustomBoxCollider collider = GetComponent<CustomBoxCollider>();

        if (collider != null)
            collider.CollisionContact -= HandleContact;

        horizontalInput = 0f;
        verticalSpeed = 0f;
        jumpRequested = false;
        isGrounded = false;
    }

    private void Update()
    {
        horizontalInput = moveAction.action.ReadValue<Vector2>().x;

        if (jumpAction.action.WasPressedThisFrame())
            jumpRequested = true;
    }

    private void FixedUpdate()
    {
        if (jumpRequested && isGrounded)
            verticalSpeed = jumpSpeed;

        jumpRequested = false;
        isGrounded = false;

        verticalSpeed -= gravity * Time.fixedDeltaTime;

        Vector3 movement = new Vector3(
            horizontalInput * moveSpeed,
            verticalSpeed,
            0f
        );

        transform.position += movement * Time.fixedDeltaTime;
    }

    private void HandleContact(CustomCollider other, Vector3 contactDirection)
    {
        if (contactDirection.y > 0f && verticalSpeed <= 0f)
        {
            isGrounded = true;
            verticalSpeed = 0f;
        }
        else if (contactDirection.y < 0f && verticalSpeed > 0f)
        {
            verticalSpeed = 0f;
        }
    }
}