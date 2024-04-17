using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public struct Pool
    {
        public string poolName;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.poolName, objectPool);
        }
    }

    public GameObject GetFromPool(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogError("Pool with name " + poolName + " does not exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[poolName];
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Optionally expand the pool here if needed
            foreach (var poolItem in pools)
            {
                if (poolItem.poolName == poolName)
                {
                    GameObject obj = Instantiate(poolItem.prefab);
                    return obj;
                }
            }

            Debug.LogError("No prefab available for pool name: " + poolName);
            return null;
        }
    }

    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogError("Pool with name " + poolName + " does not exist.");
            Destroy(obj); // or handle it some other way
            return;
        }

        obj.SetActive(false);
        poolDictionary[poolName].Enqueue(obj);
    }
}