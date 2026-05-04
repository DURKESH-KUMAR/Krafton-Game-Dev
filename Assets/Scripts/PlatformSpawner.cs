using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject Platform;     //platform prefab
    Queue<GameObject> platformPool = new Queue<GameObject>();       //queue for object pooling
    public int poolSize;
    public float minHeightDiff,maxHeightDiff,maxPosX;       //parameters for randomizing the platform pos with limitations
    Vector2 nextPlatformSpawnPoint = new Vector2(2,-2);
    void Start()
    {
        for(int i = 0; i < poolSize; i++)       //spawning platforms and moving into the pool
        {
            GameObject platform=Instantiate(Platform, transform);
            platform.SetActive(false);
            platformPool.Enqueue(platform);
        }
        for(int i = 0; i < 10; i++)     //spawn some platforms at the begining
        {
            SpawnPlatform(nextPlatformSpawnPoint);
            nextPlatformSpawnPoint =new Vector2(Random.Range(-maxPosX, maxPosX),nextPlatformSpawnPoint.y+ Random.Range(minHeightDiff, maxHeightDiff));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlatform(Vector2 pos)
    {
        GameObject platform = platformPool.Dequeue();
        platform.transform.position = pos;
        platform.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            collision.gameObject.SetActive(false);
            platformPool.Enqueue(collision.gameObject);     //remove the old platforms from world and move the platforms to pool

        }
    }
}
