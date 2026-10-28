using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Camera cam;
    private float playerY;

    private void Start()
    {
        cam = GetComponent<Camera>();
        playerY = player.position.y;
    }

    private void LateUpdate()
    {
        Vector3 vpPos = cam.WorldToViewportPoint(player.position);

        float clampedX = Mathf.Clamp01(vpPos.x);
        float clampedY = Mathf.Clamp01(vpPos.y);

        // If neither coordinate changed, the player is still inside the viewport.
        if (clampedX == vpPos.x &&
            clampedY == vpPos.y)
        {
            return;
        }

        player.position = ViewportToGround(clampedX, clampedY);
    }

    /// <summary>
    /// Converts a viewport position into a world position on the player's horizontal plane.
    /// </summary>
    /// <param name="x">The horizontal viewport coordinate.</param>
    /// <param name="y">The vertical viewport coordinate.</param>
    /// <returns>The corresponding world position at the player's Y position.</returns>
    private Vector3 ViewportToGround(float x, float y)
    {
        Vector3 pt = cam.ViewportToWorldPoint(
            new Vector3(x, y, 1f));

        Vector3 dir = pt - transform.position;

        /*
         * Find where the camera direction intersects the player's
         * horizontal plane so the corrected position keeps the same Y.
         */
        float dist = (playerY - transform.position.y) / dir.y;

        return transform.position + dir * dist;
    }
}