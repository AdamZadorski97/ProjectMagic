using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Serialization;

public class ObjectSpawner : MonoBehaviour
{
    // Singleton instance
    public static ObjectSpawner Instance { get; private set; }

    public List<PrefabInfo> prefabList;

    private void Awake()
    {
        // Ensure only one instance of ObjectSpawner exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of ObjectSpawner already exists. Destroying this one.");
            Destroy(gameObject);
        }
    }

    public void SpawnObject(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject prefab = FindPrefabByName(prefabName);

        if (prefab != null)
        {
            // Instantiate the prefab with given position, rotation, and scale
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.transform.localScale = scale;
        }
        else
        {
            Debug.LogError("Prefab with name " + prefabName + " not found!");
        }
    }

    // Method to find a prefab by name
    private GameObject FindPrefabByName(string name)
    {
        foreach (PrefabInfo prefabInfo in prefabList)
        {
            if (prefabInfo.name == name)
            {
                return prefabInfo.prefab;
            }
        }
        return null; // Return null if no prefab found with the given name
    }
}

[Serializable]
public class PrefabInfo
{
    [OdinSerialize]
    public string name;
    public GameObject prefab;
}