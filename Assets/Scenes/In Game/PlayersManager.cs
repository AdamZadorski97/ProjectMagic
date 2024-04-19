using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance { get; private set; }

    public r_CharacterConfig myPlayer;
    public List<r_CharacterConfig> otherPlayers = new List<r_CharacterConfig>();
    public List<r_CharacterConfig> allPlayers = new List<r_CharacterConfig>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
