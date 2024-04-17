using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private List<GameObject> SS = new List<GameObject>();
    private List<GameObject> Jude = new List<GameObject>();

    [SerializeField] private Transform[] SpawnPoint;

    [SerializeField] private GameObject[] JudePrefab;
    [SerializeField] private GameObject[] SSPrefab;

    [SerializeField] private byte JudeCount;
    [SerializeField] private byte SSCount;
    



    private void InitNPC()
    {

    }


}
