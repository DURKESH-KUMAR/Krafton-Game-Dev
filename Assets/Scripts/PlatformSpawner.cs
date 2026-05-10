using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefab & Pool")]
    public GameObject platformPrefab;
    public int poolSize = 20;

    [Header("Spawn Parameters")]
    public float minHeightDiff = 1.5f;
    public float maxHeightDiff = 3.5f;
    public float maxPosX = 3f;

    [Header("Camera / Ball Reference")]
    public Transform ballTransform;         // drag Ball here in Inspector
    public float spawnAheadDistance = 10f;  // spawn new platforms this far above ball
    public float despawnBehindDistance = 10f; // despawn platforms this far below ball

    // Internals
    private Queue<GameObject> platformPool = new Queue<GameObject>();
    private List<GameObject> activePlatforms = new List<GameObject>();
    private Vector2 nextSpawnPoint = new Vector2(0f, -2f);

    void Start()
    {
        // Build the object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject p = Instantiate(platformPrefab, transform);
            p.SetActive(false);
            platformPool.Enqueue(p);
        }

        // Seed initial platforms
        for (int i = 0; i < 12; i++)
        {
            SpawnPlatform(nextSpawnPoint);
            AdvanceSpawnPoint();
        }
    }

    void Update()
    {
        if (ballTransform == null) return;

        float ballY = ballTransform.position.y;

        // Spawn new platforms ahead of the ball
        while (nextSpawnPoint.y < ballY + spawnAheadDistance)
        {
            SpawnPlatform(nextSpawnPoint);
            AdvanceSpawnPoint();
        }

        // Recycle platforms that fell too far below the ball
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            GameObject p = activePlatforms[i];
            if (p.transform.position.y < ballY - despawnBehindDistance)
            {
                p.SetActive(false);
                platformPool.Enqueue(p);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    // ─── Spawn one platform at pos ───────────────────────────────────
    void SpawnPlatform(Vector2 pos)
    {
        if (platformPool.Count == 0)
        {
            // Pool exhausted: grow it by one (safety net)
            GameObject extra = Instantiate(platformPrefab, transform);
            extra.SetActive(false);
            platformPool.Enqueue(extra);
            Debug.LogWarning("[PlatformSpawner] Pool exhausted — consider increasing poolSize.");
        }

        GameObject platform = platformPool.Dequeue();
        platform.transform.position = pos;
        platform.SetActive(true);
        activePlatforms.Add(platform);
    }

    // ─── Advance the next spawn cursor upward ───────────────────────
    void AdvanceSpawnPoint()
    {
        nextSpawnPoint = new Vector2(
            Random.Range(-maxPosX, maxPosX),
            nextSpawnPoint.y + Random.Range(minHeightDiff, maxHeightDiff)
        );
    }
}