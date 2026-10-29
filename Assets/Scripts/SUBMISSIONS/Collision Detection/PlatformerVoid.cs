using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CustomBoxCollider))]
public class PlatformerVoid : MonoBehaviour
{
    private bool isRestarting;

    private void OnEnable()
    {
        GetComponent<CustomBoxCollider>().TriggerEnter += HandleEnter;
    }

    private void OnDisable()
    {
        CustomBoxCollider collider = GetComponent<CustomBoxCollider>();

        if (collider != null)
            collider.TriggerEnter -= HandleEnter;
    }

    private void HandleEnter(CustomCollider other)
    {
        if (isRestarting)
            return;

        Component otherComponent = other as Component;

        if (otherComponent == null || otherComponent.GetComponent<PlayerPlatformer>() == null)
        {
            return;
        }

        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}