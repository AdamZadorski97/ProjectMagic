using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using InControl;
using UnityEngine.TextCore.Text;

public class r_InGameController : MonoBehaviour
{
    public static r_InGameController instance;
    #region Variables
    [Header("Player Prefab")]
    public GameObject m_PlayerPrefab;

    [Header("SpawnPoints")]
    public bool m_Spawned;
    public Transform[] m_SpawnPoints;



    public Camera gameCamera;

    #endregion

    #region Unity Calls
    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;
        HandleButtons();
    }

    private void HandleButtons()
    {
        CheckMasterClient();
    }
    #endregion

    /// <summary>
    /// If a player left the room, we are checking the masterclient again and making the button interactable or not.
    /// </summary>

    #region Masterclient
    public void CheckMasterClient()
    {

    }
    #endregion

    /// <summary>
    /// Spawning the player
    /// </summary>

    #region Spawning


    private void Start()
    {
        CheckMasterClient();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

        if (m_Spawned) return;

        Transform m_SpawnPoint = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)];

        if (m_SpawnPoint)
        {
            GameObject _Player = (GameObject)PhotonNetwork.Instantiate("Player/" + m_PlayerPrefab.name, m_SpawnPoint.position, m_SpawnPoint.rotation, 0);
            gameCamera = _Player.GetComponent<FirstPersonController>().playerCamera;
            _Player.GetComponent<r_CharacterConfig>().m_PhotonView.RPC("SyncValues", RpcTarget.AllBuffered, PlayersManager.Instance.allPlayers.Count);
            _Player.GetComponent<r_CharacterConfig>().SetupLocalPlayer();
            _Player.GetComponent<r_CharacterConfig>().SetupOtherPlayer();


        }

        m_Spawned = true;
    }
    #endregion

    /// <summary>
    /// Leave the current room.
    /// </summary>

    #region Room
    private IEnumerator EndGame()
    {
        Hashtable _State = new Hashtable(); _State.Add("RoomState", "InLobby");
        PhotonNetwork.CurrentRoom.SetCustomProperties(_State);

        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(0);
    }

    private void LeaveRoom() => PhotonNetwork.LeaveRoom();
    #endregion
}