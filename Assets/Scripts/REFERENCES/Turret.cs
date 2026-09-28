using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float range = 5f;
    [SerializeField] private float detectionAngle = 60f;

    private void Update()
    {
        if (target == null) return;
        
        if (Object3DDetector.IsInCone(transform, 
            target.transform,
            range,
            detectionAngle
            ))
        {
            LookAtObject();
        }
    }

    private void LookAtObject()
    {
        Vector3 dir = target.position - transform.position; 
        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
