using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 2D Player Controller for 2D Unity Games
/// </summary>
public class Player2DController : MonoBehaviour, IHittable2D
{
#region Inspector Fields
    [Header("Input")]
    [SerializeField] private InputActionReference move;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Hitbox")]
    [SerializeField] private float hitRadius = 0.5f;
#endregion

    private bool isHit;
    private bool isInvincible = false;

    public Vector2 HitPosition => transform.position;
    public bool IsInvincible { get; set; }    
    public float HitRadius => hitRadius;

#region Unity Life Cycle
    private void OnEnable()
    {
        move.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
    }

    private void Update()
    {
        Move();
    }
    
#endregion

#region Player Movement
    private void Move()
    {
        Vector2 input = move.action.ReadValue<Vector2>();
        Vector3 movement = transform.right * input.x + transform.up * input.y;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
#endregion

#region Hit Logic
    public void Hit(PooledGun sourceGun)
    {
        if (isHit || isInvincible) return;
        
        isHit = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
#endregion
}
