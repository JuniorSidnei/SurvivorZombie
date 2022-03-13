using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using SurvivorZombies.Server;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Player {
    
    public class PlayerSpawner : MonoBehaviour {
        public GameObject Player;
        public GameObject Zombie;
        public LayerMask ObstacleLayer;
        
        public Transform spawnTransform;
        public List<Transform> zombiesSpawns;
        public float zombieSpawnTimer;
        
        public delegate void OnPlayerSpawned();
        public static event OnPlayerSpawned onPlayerSpawned;
        
        private float m_zombieSpawnTimer;
        private PhotonView m_photonView;
        private int m_maxZombiesInScene = 20;
        private int m_zombiesInScene = 0;
        private List<Transform> m_players = new List<Transform>();

        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            if (!m_photonView.IsMine) return;
            m_photonView.RPC("CreatePlayer", RpcTarget.All);

            m_zombieSpawnTimer = zombieSpawnTimer;
        }

        private void Update() {
            if (!m_photonView.IsMine) return;
            m_zombieSpawnTimer -= Time.deltaTime;
            if (!(m_zombieSpawnTimer <= 0)) return;
            
            m_zombieSpawnTimer = zombieSpawnTimer;
            m_photonView.RPC("CreateZombie", RpcTarget.All);
        }

        private void SpawnZombie() {
            if (PhotonNetwork.CurrentRoom == null) {
                return;
            }
            
            var random = Random.Range(0, zombiesSpawns.Count);
            var zombie = PhotonNetwork.InstantiateRoomObject(Zombie.name, zombiesSpawns[random].position, Quaternion.identity);
            var target = FindObjectOfType<PlayerInput>().transform;
            zombie.GetComponent<TargetSeeker>().SetTarget(target);
        }


        [PunRPC]
        private void CreatePlayer() {
            var player = PhotonNetwork.Instantiate(Player.name, spawnTransform.position, Quaternion.identity);
            onPlayerSpawned?.Invoke();
            m_players.Add(player.transform);
        }

        [PunRPC]
        private void CreateZombie() {
            if (m_zombiesInScene >= m_maxZombiesInScene) return;
            
            var random = Random.Range(0, zombiesSpawns.Count);
            if (Physics.Raycast(zombiesSpawns[random].position, Vector3.down, 10f, ObstacleLayer)) {
                m_photonView.RPC("CreateZombie", RpcTarget.All);
                return;
            }
            
            m_zombiesInScene++;
            var zombie = PhotonNetwork.InstantiateRoomObject(Zombie.name, zombiesSpawns[random].position, Quaternion.identity);
            var randomTarget = Random.Range(0, m_players.Count);
            zombie.GetComponent<TargetSeeker>().SetTarget(m_players[randomTarget]);
        }
    }
}