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

        if (clampedX == vpPos.x &&
            clampedY == vpPos.y)
        {
            return;
        }

        player.position = ViewportToGround(clampedX, clampedY);
    }

    private Vector3 ViewportToGround(float x, float y)
    {
        Vector3 pt = cam.ViewportToWorldPoint(
            new Vector3(x, y, 1f));
        
        Vector3 dir = pt - transform.position;

        float dist = (playerY - transform.position.y) / dir.y;

        return transform.position + dir * dist;
    }
}
