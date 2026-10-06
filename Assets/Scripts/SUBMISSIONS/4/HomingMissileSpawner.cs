using UnityEngine;

public class HomingMissileSpawner : MonoBehaviour
{
#region Unity inspector Fields
    [SerializeField] private SimplePlayer player;
    [SerializeField] private float spawnOffset = -10f;
    [SerializeField] private HomingMissile missilePrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float spawnSpread = 5f;
#endregion

#region Runtime Properties
    private float spawnTimer;
    private int missileCount = 1;
#endregion

#region Unity Life Cycle
    private void Start()
    {
        SpawnMissiles();
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            missileCount++;
            SpawnMissiles();
            spawnTimer = spawnInterval;
        }
    }
#endregion

#region Spawning Logic
    private void SpawnMissiles()
    {
        for (int i = 0; i < missileCount; i++)
        {
            Vector3 spawnPos =
                player.transform.position
                + player.transform.forward * spawnOffset
                + player.transform.right * Random.Range(-spawnSpread, spawnSpread);

            HomingMissile missile = Instantiate(
                missilePrefab,
                spawnPos,
                missilePrefab.transform.rotation
            );

            missile.SetTarget(player);
        }
    }
#endregion
}
