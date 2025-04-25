using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject[] roadPrefabs; // Nhiều prefab đoạn đường

    public int numberOfTiles = 5;         // Số lượng đoạn đường spawn trước
    public float tileLength = 10f;        // Chiều dài mỗi đoạn đường (theo X)
    public Transform player;             // Tham chiếu tới Player

    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnX = 0.0f;          // Vị trí X để spawn đoạn mới
    private float safeZone = 20.0f;       // Khoảng cách an toàn trước khi spawn đoạn mới

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // Chỉ xóa tile khi player đã vượt qua tile thứ 2 (index 2 trở lên)
        float deleteThresholdX = activeTiles[2].transform.position.x + tileLength;

        if (player.position.x - safeZone > (spawnX - numberOfTiles * tileLength) && player.position.x > deleteThresholdX)
        {
            SpawnTile();
            DeleteTile();
        }
    }


    void SpawnTile()
    {
        int randomIndex = Random.Range(0, roadPrefabs.Length);
        GameObject tile = Instantiate(roadPrefabs[randomIndex], new Vector3(spawnX, 0, 0), Quaternion.identity);
        activeTiles.Add(tile);
        spawnX += tileLength;
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    // --- Vẽ Gizmo để đo chiều dài tile ---
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + Vector3.right * tileLength;

        // Vẽ đường thẳng đỏ
        Gizmos.DrawLine(startPoint, endPoint);

        // Vẽ một cái box mờ để dễ nhìn
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Màu đỏ nhạt
        Gizmos.DrawCube(startPoint + Vector3.right * (tileLength / 2f), new Vector3(tileLength, 1, 5)); 
        // 5 là độ rộng đường (có thể chỉnh nếu đường bạn to nhỏ khác nhau)
    }
}
