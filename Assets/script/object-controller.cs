using UnityEngine;

public class PrefabSpawnController : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnDelay = 5f;
    private float timer = 0f;
    private bool shouldSpawn = false;
    private Vector3 spawnPosition;

    void Update()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf && !shouldSpawn)
            {
                shouldSpawn = true;
                spawnPosition = child.position;
                timer = 0f;
            }
        }

        if (shouldSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                SpawnPrefab();
                shouldSpawn = false;
            }
        }
    }

    void SpawnPrefab()
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}