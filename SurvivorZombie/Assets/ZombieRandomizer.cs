using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SurvivorZombies.Zombies {

    public class ZombieRandomizer : MonoBehaviour {

        public List<GameObject> ZombieSkins = new List<GameObject>();

        private PhotonView m_photonView;
        
        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            if (!m_photonView.IsMine) return;
            var random = Random.Range(0, ZombieSkins.Count);
            ZombieSkins[random].gameObject.SetActive(true);
        }
    }
}