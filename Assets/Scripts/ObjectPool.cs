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
    private Dictionary<string, Transform> poolParentDictionary; // To hold the parent transforms

    private void Awake()
    {
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolParentDictionary = new Dictionary<string, Transform>(); // Initialize the parent dictionary

        foreach (var pool in pools)
        {
            // Create a parent GameObject for each pool
            GameObject poolHolder = new GameObject(pool.poolName + " Pool");
            poolHolder.transform.SetParent(transform); // Set as child of the ObjectPool GameObject

            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(poolHolder.transform); // Parent the pooled object to its pool's holder
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.poolName, objectPool);
            poolParentDictionary.Add(pool.poolName, poolHolder.transform); // Store the parent transform
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
            obj.transform.SetParent(null); // Optionally unparent when taken from the pool
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
                    obj.transform.SetParent(poolParentDictionary[poolName]); // Parent to its holder
                    return obj;
                }
            }

            Debug.LogError("No prefab available for pool name: " + poolName);
            return null;
        }
    }

    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolName) || !poolParentDictionary.ContainsKey(poolName))
        {
            Debug.LogError("Pool with name " + poolName + " does not exist.");
            Destroy(obj); // or handle it some other way
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(poolParentDictionary[poolName]); // Re-parent to its holder
        poolDictionary[poolName].Enqueue(obj);
    }
}