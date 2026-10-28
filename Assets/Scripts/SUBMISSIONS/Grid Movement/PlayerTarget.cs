using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    [SerializeField]
    public Transform player;

    [SerializeField]
    public Transform target;

    void Update()
    {
        if (player == null || target == null)
            return;
        
        var dir = target.position - player.position;

        // if (dir.magnitude <= 3)
        //     Debug.Log("Enemy Detected");

        Debug.Log(dir.normalized);
    }
}
