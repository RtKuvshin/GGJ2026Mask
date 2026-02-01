using UnityEngine;

public class SingleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject prefabToSpawn;   // prefab to spawn
    public Transform[] spawnPoints;    // possible spawn locations

    private GameObject spawnedObject;

    void Start()
    {
        SpawnOnce();
    }

    /// <summary>
    /// Spawns the prefab once at a random location
    /// </summary>
    public void SpawnOnce()
    {
        if (spawnedObject != null) return; // already spawned
        if (prefabToSpawn == null || spawnPoints.Length == 0)
        {
            Debug.LogError("SingleSpawner: Assign prefabToSpawn and at least one spawnPoint!");
            return;
        }

        // Pick a random spawn point
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        // Spawn prefab
        spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}
