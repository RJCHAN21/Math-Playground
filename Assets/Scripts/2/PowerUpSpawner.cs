using System.Collections;
using UnityEngine;

/// <summary>
/// Periodically spawns a random power-up prefab at a random visible
/// position within the camera viewport on a fixed Y plane.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs; // You can add more in the future
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private float spawnInterval = 3f;

    private void Start()
    {
        StartCoroutine(StartSpawningPowerUps());
    }

    private IEnumerator StartSpawningPowerUps()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomPowerUp();
        }
    }

    /// <summary>
    /// Selects a random power-up and spawns it at a random point
    /// visible through the camera on the configured spawn height.
    /// </summary>
    private void SpawnRandomPowerUp()
    {
        if (prefabs.Length == 0 || targetCamera == null)
            return;

        int randIndex = Random.Range(0, prefabs.Length);

        // Choose a random position within the camera viewport.
        Vector3 vpPos = new Vector3(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            0f
        );
        
        Ray ray = targetCamera.ViewportPointToRay(vpPos);
        
        // Makes a horizontal place based on spawn height.
        Plane spawnPlane = new Plane(
            Vector3.up, 
            new Vector3(0f, spawnHeight, 0f));

        // Finds where the viewport ray intersects the spawn plane.
        if (spawnPlane.Raycast(ray, out float distance))
        {
            Vector3 spawnPos = ray.GetPoint(distance);

            Instantiate(
                prefabs[randIndex],
                spawnPos,
                Quaternion.identity
            );
        }
    }
}
