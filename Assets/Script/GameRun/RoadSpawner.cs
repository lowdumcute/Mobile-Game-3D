using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject[] roadPrefabs;
    public int numberOfTiles = 5;
    public float tileLength = 10f;
    public Transform player;

    private List<GameObject> activeTiles = new List<GameObject>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();

    private float spawnX = 0f;
    private float safeZone = 20f;

    public void CreatedRoad()
    {
        spawnX = 0;
        for (int i = 0; i < numberOfTiles; i++)
        {
            GameObject tile = GetTileFromPool();
            tile.transform.position = new Vector3(spawnX, 0, 0);
            tile.SetActive(true);
            activeTiles.Add(tile);

            // Gọi spawn obstacle
            TrySpawnObstacles(tile);

            spawnX += tileLength;
        }
    }

    public void ResetRoad()
    {
        foreach (GameObject tile in activeTiles)
        {
            if (tile != null)
            {
                // Làm sạch obstacle bên trong tile trước khi ẩn nó
                ObstacleSpawner spawner = tile.GetComponentInChildren<ObstacleSpawner>();
                if (spawner != null)
                {
                    spawner.ClearObstacles();
                }

                tile.SetActive(false);
                tilePool.Enqueue(tile);  // Đưa lại vào pool
            }
        }

        activeTiles.Clear();
        CreatedRoad();
    }


    void Update()
    {
        if (activeTiles.Count < 3) return;
        // Check null an toàn
        if (activeTiles[2] == null) return;

        float deleteThresholdX = activeTiles[2].transform.position.x + tileLength;

        if (player.position.x - safeZone > (spawnX - numberOfTiles * tileLength) && player.position.x > deleteThresholdX)
        {
            ReuseTile();
        }
    }

    void ReuseTile()
    {
        GameObject tile = activeTiles[0];
        activeTiles.RemoveAt(0);

        tile.transform.position = new Vector3(spawnX, 0, 0);
        tile.SetActive(true);
        activeTiles.Add(tile);

        // Gọi spawn obstacle khi tile di chuyển về phía trước
        TrySpawnObstacles(tile);

        spawnX += tileLength;
    }

    GameObject GetTileFromPool()
    {
        GameObject tile;

        if (tilePool.Count > 0)
        {
            tile = tilePool.Dequeue();
        }
        else
        {
            int randomIndex = Random.Range(0, roadPrefabs.Length);
            tile = Instantiate(roadPrefabs[randomIndex]);
        }

        return tile;
    }

    void TrySpawnObstacles(GameObject tile)
    {
        ObstacleSpawner spawner = tile.GetComponent<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.SpawnObstacles();
        }
    }

    public List<GameObject> GetActiveTiles()
    {
        return activeTiles;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + Vector3.right * tileLength;

        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(startPoint + Vector3.right * (tileLength / 2f), new Vector3(tileLength, 1, 5));
    }
}
