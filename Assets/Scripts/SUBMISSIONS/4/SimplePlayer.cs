using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SimplePlayer : MonoBehaviour
{
#region Unity Inspector Fields
    [Header("Input")]
    [SerializeField] private InputActionReference turnAction;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float yawSpeed = 20f;
    [SerializeField] private float turnSpeed = 60f;
#endregion

#region Public Properties
    public float hitRadius = 1f;
    public UnityEvent onHit;
#endregion

#region Runtime Properties
    private float maxTurnAngle = 15f;
    private Quaternion headingRot;
    private int hitCount = 0;
#endregion

#region Unity Life Cycle
    private void Start()
    {
        headingRot = transform.rotation;
    }

    private void OnEnable()
    {
        turnAction.action.Enable();
        onHit.AddListener(HitPlayer);
    }

    private void OnDisable()
    {
        turnAction.action.Disable();
        onHit.RemoveListener(HitPlayer);
    }

    private void Update()
    {
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

        Vector2 input = turnAction.action.ReadValue<Vector2>();

        headingRot *= Quaternion.AngleAxis(
            input.x * yawSpeed * Time.deltaTime,
            Vector3.up
        );

        Quaternion rollRot =
            Quaternion.AngleAxis(-input.x * maxTurnAngle, Vector3.forward);

        Quaternion targetRot = headingRot * rollRot;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }
#endregion

#region Hit Logic
    public void HitPlayer()
    {
        hitCount++;

        if (hitCount < 5) return;

        Debug.Log("Player was caught by homing missile.");
        RestartGame();
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endregion
}
