using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using SurvivorZombies.Server;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SurvivorZombies.Utils {
    
    public class RoomManager : MonoBehaviourPunCallbacks {
        
        private PlayerRoomManager m_playerRoomManager;
        private ZombiesRoomManager m_zombieRoomManager;
        private PhotonView m_photonView;
        
        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            if (!m_photonView.IsMine) return;
            m_photonView.RPC(nameof(m_playerRoomManager.CreatePlayer), RpcTarget.All);
            m_zombieRoomManager.SetTargets(m_playerRoomManager.Players);
        }
        
        
        public override void OnPlayerLeftRoom(Photon.Realtime.Player other  ) {
            if (PhotonNetwork.IsMasterClient) {
                SceneManager.LoadScene("Lobby"); 
            }
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene("Lobby");
        }
    }
}