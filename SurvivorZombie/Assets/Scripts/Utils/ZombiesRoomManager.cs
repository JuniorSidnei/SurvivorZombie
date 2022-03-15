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
        private List<Transform> m_targetList = new List<Transform>();
        
        public void SetTargets(List<Transform> targets) {
            m_targetList = targets;
        }
        
        [PunRPC]
        public void CreateZombie() {
            var random = Random.Range(0, zombiesSpawns.Count);
            if (Physics.Raycast(zombiesSpawns[random].position, Vector3.down, 10f, ObstacleLayer)) {
                return;
            }
            
            var zombie = PhotonNetwork.InstantiateRoomObject(Zombie.name, zombiesSpawns[random].position, Quaternion.identity);
            var randomTarget = Random.Range(0, m_targetList.Count - 1);
            zombie.GetComponent<ZombieController>().SetTarget(m_targetList[randomTarget]);
        }
    }
}