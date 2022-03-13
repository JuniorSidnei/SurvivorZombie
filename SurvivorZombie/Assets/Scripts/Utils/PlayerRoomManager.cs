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
        
        public delegate void OnPlayerSpawned();
        public static event OnPlayerSpawned onPlayerSpawned;
        
        private List<Transform> m_players = new List<Transform>();

        public List<Transform> Players => m_players;

        [PunRPC]
        public void CreatePlayer() {
            var player = PhotonNetwork.Instantiate(Player.name, spawnTransform.position, Quaternion.identity);
            onPlayerSpawned?.Invoke();
            m_players.Add(player.transform);
        }
    }
}