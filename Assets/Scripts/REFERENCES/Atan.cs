using UnityEngine;

public class Atan : MonoBehaviour
{
    [SerializeField] private Transform enemy;

    void Update()
    {
        if (enemy == null) return;

        var dir = enemy.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Slerp(transform.rotation,
        //     Quaternion.Euler(0, angle, 0), 
        //     rotSpeed * Time.deltaTime);

        var dot = Vector3.Dot(transform.forward.normalized, dir.normalized);
        Debug.Log(dot);
    }
}
