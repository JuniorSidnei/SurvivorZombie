using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Server;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Player {
    
    public class PlayerSpawner : MonoBehaviour {
        public GameObject Player;
        public GameObject Zombie;
        
        public Transform spawnTransform;
        public List<Transform> zombiesSpawns;
        public float zombieSpawnTimer;
        
        public delegate void OnPlayerSpawned();
        public static event OnPlayerSpawned onPlayerSpawned;
        
        private float m_zombieSpawnTimer;
        
        private void Start() {
            PhotonNetwork.Instantiate(Player.name, spawnTransform.position, Quaternion.identity);
            onPlayerSpawned?.Invoke();
            m_zombieSpawnTimer = zombieSpawnTimer;
        }

        private void Update() {
            m_zombieSpawnTimer -= Time.deltaTime;
            if (!(m_zombieSpawnTimer <= 0)) return;
            
            m_zombieSpawnTimer = zombieSpawnTimer;
            SpawnZombie();
        }

        private void SpawnZombie() {
            if (PhotonNetwork.CurrentRoom == null) {
                return;
            }
            
            var random = Random.Range(0, zombiesSpawns.Count);
            var zombie = PhotonNetwork.Instantiate(Zombie.name, zombiesSpawns[random].position, Quaternion.identity);
            var target = FindObjectOfType<PlayerInput>().transform;
            zombie.GetComponent<TargetSeeker>().SetTarget(target);
        }
    }
}