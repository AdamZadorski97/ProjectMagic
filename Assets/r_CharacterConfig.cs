using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;
using InControl;

public class r_CharacterConfig : MonoBehaviourPunCallbacks
{
    #region Variables
    [Header("Photon View")]
    public TMP_Text playerName;

    public Camera playerCamera;

    [Header("Photon View")]
    public PhotonView m_PhotonView;

    [Header("MonoBehaviours")]
    public MonoBehaviour[] m_LocalScripts;

    [Header("Objects To Enable")]
    public GameObject[] m_LocalObjects;

    [Header("Objects To Disable")]
    public GameObject[] m_RemoteObjects;

    [Header("Remote MonoBehaviours To Disable")]
    public MonoBehaviour[] m_RemoteScripts;

    public List<GameObject> avatars = new List<GameObject>();



    #endregion

    /// <summary>
    /// Here we enabling or disabling the local and remote objects and scripts.
    /// We set our username on our transform.
    /// </summary>

    #region Setup Player
    public void SetupLocalPlayer()
    {
        if (!m_PhotonView.IsMine) return;

        foreach (MonoBehaviour _LocalScript in m_LocalScripts) _LocalScript.enabled = true;

        foreach (GameObject _Object in m_LocalObjects) _Object.SetActive(true);


        PlayersManager.Instance.myPlayer = this;

        m_PhotonView.RPC("SetupPlayerName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        m_PhotonView.RPC("SetupPlayerNameText", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        GetComponent<FirstPersonController>().SetID(1, InputManager.Devices[0]);
    }

    public void SetupOtherPlayer()
    {
     if (m_PhotonView.IsMine) return;
        foreach (MonoBehaviour _Object in m_RemoteScripts) _Object.enabled = false;
    }


    [PunRPC]
    public void SetupPlayerName(string _PlayerName) => this.transform.name = _PlayerName;
    [PunRPC]
    public void SetupPlayerNameText(string _PlayerName) => playerName.text = _PlayerName;
    #endregion
}
