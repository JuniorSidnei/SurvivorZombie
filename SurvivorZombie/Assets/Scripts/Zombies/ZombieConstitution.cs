using System.Collections;
using System.Collections.Generic;
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
        
        public delegate void OnDeath();
        public static event OnDeath onDeath;
        
        private void Start() {
            m_currentHealth = characterData.MaxHealth;
        }
        
        public void Damage(float damage) {
            if (m_currentHealth <= 0) return;
            onHurt?.Invoke();
            m_currentHealth -= damage;
            UpdateLife();
        }

        public void UpdateLife() {
            health.fillAmount = m_currentHealth / characterData.MaxHealth;
            if (!(m_currentHealth <= 0)) return;
            
            m_currentHealth = 0;
            healthCanvas.SetActive(false);
            onDeath?.Invoke();
        }
    }
}