using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;                // Các điểm spawn
    public string[] obstaclePrefabNames;           // Tên prefab trong ObstaclePooler

    public int maxObstaclesPerTile = 2;            // Tối đa số chướng ngại vật mỗi tile

    public void SpawnObstacles()
    {
        if (spawnPoints.Length == 0 || obstaclePrefabNames.Length == 0) return;

        // Trả toàn bộ obstacle con về pool (ẩn tất cả obstacles ở mọi spawnPoints)
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnPoint.childCount; i++)
            {
                spawnPoint.GetChild(i).gameObject.SetActive(false);
            }
        }

        // Tạo danh sách vị trí spawn và trộn ngẫu nhiên
        List<int> spawnIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnIndices.Add(i);
        }

        ShuffleList(spawnIndices);

        // Spawn tối đa maxObstaclesPerTile
        for (int i = 0; i < Mathf.Min(maxObstaclesPerTile, spawnPoints.Length); i++)
        {
            int spawnIndex = spawnIndices[i];
            Transform spawnPoint = spawnPoints[spawnIndex];

            string prefabName = obstaclePrefabNames[Random.Range(0, obstaclePrefabNames.Length)];

            GameObject obstacle = ObstaclePooler.Instance.GetFromPool(prefabName);
            if (obstacle != null)
            {
                // Giữ nguyên giá trị Y cũ
                Vector3 spawnPos = spawnPoint.position;
                spawnPos.y = obstacle.transform.position.y;

                obstacle.transform.position = spawnPos;
                obstacle.transform.SetParent(spawnPoint);

                obstacle.SetActive(true);
            }

        }
    }

    // Trộn danh sách chỉ số spawn (Fisher-Yates shuffle)
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    public void ClearObstacles()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = spawnPoint.childCount - 1; i >= 0; i--)
            {
                Transform child = spawnPoint.GetChild(i);
                child.SetParent(null); // Tách khỏi spawnPoint
                child.gameObject.SetActive(false); // Ẩn để trả lại pool
            }
        }
    }

}
