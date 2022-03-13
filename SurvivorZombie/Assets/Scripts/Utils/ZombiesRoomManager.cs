using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Utils {
    
    public class ZombiesRoomManager : MonoBehaviour
    {
        public GameObject Zombie;
        public LayerMask ObstacleLayer;
        public List<Transform> zombiesSpawns;
        public float zombieSpawnTimer;
        public PhotonView PhotonView;
        
        private float m_zombieSpawnTimer;
        private int m_maxZombiesInScene = 20;
        private int m_zombiesInScene = 0;
        private bool m_canSpawnZombie;
        private List<Transform> m_targetList = new List<Transform>();
        

        private void Start() {
            m_zombieSpawnTimer = zombieSpawnTimer;
        }
        
        private void Update() {
            m_zombieSpawnTimer -= Time.deltaTime;
            if (!(m_zombieSpawnTimer <= 0)) return;
            
            PhotonView.RPC("CreateZombie", RpcTarget.All);
            m_zombieSpawnTimer = zombieSpawnTimer;
        }

        public void SetTargets(List<Transform> targets) {
            m_targetList = targets;
        }
        
        [PunRPC]
        private void CreateZombie() {
            if (m_zombiesInScene >= m_maxZombiesInScene) return;
            
            var random = Random.Range(0, zombiesSpawns.Count);
            if (Physics.Raycast(zombiesSpawns[random].position, Vector3.down, 10f, ObstacleLayer)) {
                return;
            }
            
            m_zombiesInScene++;
            var zombie = PhotonNetwork.InstantiateRoomObject(Zombie.name, zombiesSpawns[random].position, Quaternion.identity);
            var randomTarget = Random.Range(0, m_targetList.Count);
            zombie.GetComponent<TargetSeeker>().SetTarget(m_targetList[randomTarget]);
        }
    }
}