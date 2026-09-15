using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple player controller script that uses Unity's newer input system. Note that this is strictly used as a personal preference and to avoid code deprecation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.1f;

    private float cameraPitch;
    
    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
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
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;

        transform.position += movement * moveSpeed * Time.deltaTime;

        #endregion

        #region Camera Controls

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity);

        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        #endregion
    }
}