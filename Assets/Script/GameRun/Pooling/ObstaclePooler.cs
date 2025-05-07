using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooler : MonoBehaviour
{
    public static ObstaclePooler Instance;

    public GameObject[] obstaclePrefabs;
    public int poolSizePerPrefab = 10;

    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();


    void Awake()
    {
        Instance = this;

        foreach (var prefab in obstaclePrefabs)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject obj = Instantiate(prefab, gameObject.transform); // Spawn vào trong pool parent
                obj.SetActive(false);

                // Gán obstacleName nếu có
                Obstacle obstacle = obj.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.obstacleName = prefab.name;
                }

                queue.Enqueue(obj);
            }

            poolDict.Add(prefab.name, queue);
        }
    }

    // Spawn với vị trí, xoay và parent cụ thể
    public GameObject GetFromPool(string prefabName)
    {
        if (!poolDict.ContainsKey(prefabName)) return null;

        GameObject obj = null;

        if (poolDict[prefabName].Count > 0)
        {
            obj = poolDict[prefabName].Dequeue();
        }
        else
        {
            GameObject prefab = GetPrefabByName(prefabName);
            if (prefab != null)
            {
                obj = Instantiate(prefab, gameObject.transform); // Spawn vào trong pool parent

                // Gán obstacleName nếu tạo mới
                Obstacle obstacle = obj.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.obstacleName = prefabName;
                }
            }
        }
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(gameObject.transform); // Trả về đối tượng pool chứa parent

        Obstacle obstacle = obj.GetComponent<Obstacle>();
        if (obstacle != null && poolDict.ContainsKey(obstacle.obstacleName))
        {
            poolDict[obstacle.obstacleName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy obstacleName hoặc pool tương ứng cho: " + obj.name);
        }
    }

    private GameObject GetPrefabByName(string name)
    {
        foreach (var p in obstaclePrefabs)
        {
            if (p.name == name)
                return p;
        }
        return null;
    }
}
