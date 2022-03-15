using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Player.Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Utils {
    
    public class ZombiesRoomManager : MonoBehaviour
    {
        public List<GameObject> Zombies;
        public LayerMask ObstacleLayer;
        public List<Transform> zombiesSpawns;

        [PunRPC]
        public void CreateZombie() {
            var random = Random.Range(0, zombiesSpawns.Count);
            if (Physics.Raycast(zombiesSpawns[random].position, Vector3.down, 10f, ObstacleLayer)) {
                return;
            }
            
            var randomZombie = Random.Range(0, Zombies.Count);
            var zombie = PhotonNetwork.InstantiateRoomObject(Zombies[randomZombie].name, zombiesSpawns[random].position, Quaternion.identity);
            var allPlayers = FindObjectsOfType<PlayerController>();
            var randomTarget = Random.Range(0, allPlayers.Length);
            if (zombie == null) return;
            zombie.GetComponent<ZombieController>().SetTarget(allPlayers[randomTarget].transform);
        }
    }
}