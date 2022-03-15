using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SurvivorZombies.Utils
{


    public class PlayerRoomManager : MonoBehaviour
    {
        public GameObject Player;
        public Transform spawnTransform;
        private GameObject m_playeravatar;
        
        public delegate void OnPlayerSpawned();
        public static event OnPlayerSpawned onPlayerSpawned;
        
        private List<Transform> m_players = new List<Transform>();

        public List<Transform> Players => m_players;

        [PunRPC]
        public void CreatePlayer() {
            m_playeravatar = PhotonNetwork.Instantiate(Player.name, spawnTransform.position, Quaternion.identity);
            m_players.Add(m_playeravatar.transform);
            onPlayerSpawned?.Invoke();
        }
    }
}