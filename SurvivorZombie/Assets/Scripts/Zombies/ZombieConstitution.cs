using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Zombies {

    public class ZombieConstitution : MonoBehaviour, IDamagable {
        public CharacterData characterData;
        public Image health;
        public GameObject healthCanvas;
        
        private float m_currentHealth;
        
        public delegate void OnHurt();
        public static event OnHurt onHurt;
        
        public delegate void OnDeath(GameObject gameObject);
        public static event OnDeath onDeath;

        private PhotonView m_photonView;
        
        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            m_currentHealth = characterData.MaxHealth;
        }
        
        public void Damage(float damage) {
            if (!m_photonView.IsMine) return;
            if (m_currentHealth <= 0) return;
            onHurt?.Invoke();
            m_currentHealth -= damage;
            UpdateLife();
        }

        public void UpdateLife() {
            if (!m_photonView.IsMine) return;
            health.fillAmount = m_currentHealth / characterData.MaxHealth;
            if (!(m_currentHealth <= 0)) return;
            
            m_currentHealth = 0;
            healthCanvas.SetActive(false);
            onDeath?.Invoke(gameObject);
        }
    }
}