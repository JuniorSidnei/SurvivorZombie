using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using SurvivorZombies.Server;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SurvivorZombies.Utils {
    
    public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable {
        
        public WaveManager WaveManager;
        private PlayerRoomManager m_playerRoomManager;
        private ZombiesRoomManager m_zombieRoomManager;
        private PhotonView m_photonView;
        
        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            m_playerRoomManager = GetComponent<PlayerRoomManager>();
            m_zombieRoomManager = GetComponent<ZombiesRoomManager>();
            if (!m_photonView.IsMine || !PhotonNetwork.IsMasterClient) return;
            m_photonView.RPC(nameof(m_playerRoomManager.CreatePlayer), RpcTarget.All);
            m_zombieRoomManager.SetTargets(m_playerRoomManager.Players);
        }

        private void Update() {
            if (WaveManager.CanSpawn) {
                if (!PhotonNetwork.IsMasterClient) return;
                m_photonView.RPC(nameof(m_zombieRoomManager.CreateZombie), RpcTarget.All);
                WaveManager.CanSpawn = false;
            }
        }

        public void ReloadScene() {
            m_photonView.RPC("RPC_ReloadScene", RpcTarget.All);RPC_ReloadScene();
        }

        [PunRPC]
        private void RPC_ReloadScene() {
            if (!m_photonView.IsMine || !PhotonNetwork.IsMasterClient) return;
            
            PhotonNetwork.DestroyAll();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("StreetGameScene");
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(GameManager.Instance.CurrentScore.text);
            }
            else {
                GameManager.Instance.CurrentScore.text = (string) stream.ReceiveNext();
            }
        }
    }
}