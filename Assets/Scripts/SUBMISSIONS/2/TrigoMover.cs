using UnityEngine;

public class TrigoMover : MonoBehaviour
{
    [SerializeField] private float xLength = 1f;
    [SerializeField] private float yLength = 1f;
    // [SerializeField] private float length = 1f;
    [SerializeField] private float timeMultiplier = 1f;

    private float timer;
    
    private float elapsedTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime * timeMultiplier;

        transform.position += new Vector3(
            xLength * Mathf.Cos(timer * Mathf.Deg2Rad),
            yLength * Mathf.Sin(timer * Mathf.Deg2Rad),
            0
        ) * Time.deltaTime;
            
        // transform.position += new Vector3(
        //     0,
        //     length * Mathf.Sin(elapsedTime),
        //     0 // length * Mathf.Cos(elapsedTime)
        // ) * Time.deltaTime;
    }
}
