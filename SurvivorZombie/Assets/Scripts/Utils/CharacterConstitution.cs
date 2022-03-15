using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Data;
using SurvivorZombies.Utils.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Utils {
    
    public class CharacterConstitution : MonoBehaviour {

        public CharacterData characterData;
        public Image health;
        public GameObject healthCanvas;
        
        private float m_currentHealth;
        private PhotonView m_photonView;

        protected PhotonView PhotonView => m_photonView;
        protected float CurrentHealth {
            get => m_currentHealth;
            set => m_currentHealth = value;
        }

        private void Awake() {
            m_photonView = GetComponent<PhotonView>();
            m_currentHealth = characterData.MaxHealth;
        }
    }
}