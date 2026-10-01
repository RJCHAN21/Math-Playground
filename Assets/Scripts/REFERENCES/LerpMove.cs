using UnityEngine;

public class LerpMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform controlA;
    [SerializeField] private Transform controlB;
    [SerializeField] private float timeToReachTarget = 3f;
    [SerializeField] private float resolution = 50f;

    private float totalTime;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (target == null || controlA == null || controlB == null) return;

        totalTime += Time.deltaTime;
        var lerpTime = Mathf.Clamp01(totalTime / timeToReachTarget);
        Debug.Log($"Lerped Time: {lerpTime}, totalTime:{totalTime}");

        transform.position = CubicFast(startPos, controlA.position, controlB.position, target.position, lerpTime);

        // // Change the time to change the interpolation method. Ex. Cubic, Ease In Bounce.
        // var t = Mathf.Pow(lerpTime, .5f);

        // transform.position = startPos + (target.position - startPos) * t;
    }

    [ContextMenu("Reset")]
    void Reset()
    {
        transform.position = startPos;
        totalTime = 0;
    }

    public static Vector3 QuadraticFast(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u  = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }

    public static Vector3 CubicFast(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
         return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
    }

    private void OnDrawGizmos()
    {   
        var previousLine = startPos;
        for (int i = 1; i <= resolution; i++)
        {
            var gap = i / resolution;
            var newPos = CubicFast(startPos, 
                controlA.position, 
                controlB.position, 
                target.position, 
                gap);
            Debug.DrawLine(previousLine, newPos, Color.red);
            previousLine = newPos;
        }
    }
}
