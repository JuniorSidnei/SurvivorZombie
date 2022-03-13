using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Data;
using SurvivorZombies.Utils.Sound;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Zombies {

    public class ZombieConstitution : MonoBehaviour, IDamagable, IPunObservable {
        public CharacterData characterData;
        public Image health;
        public GameObject healthCanvas;
        public List<AudioClip> GroamsSounds;
        public AudioClip HitSound;
        
        private float m_currentHealth;
        
        public delegate void OnHurt(GameObject gameObject);
        public static event OnHurt onHurt;
        
        public delegate void OnDeath(GameObject gameObject);
        public static event OnDeath onDeath;
        
        private PhotonView m_photonView;
        private float m_groamTimer;
        private float m_groamSoundTime;
        
        private void Start() {
            m_photonView = GetComponent<PhotonView>();
            m_currentHealth = characterData.MaxHealth;
            m_groamTimer = Random.Range(4, 6);
            m_groamSoundTime = m_groamTimer;
        }

        private void Update() {
            m_groamSoundTime -= Time.deltaTime;
            if (m_groamSoundTime <= 0) {
                var random = Random.Range(0, GroamsSounds.Count);
                AudioController.Instance.Play(GroamsSounds[random], AudioController.SoundType.SoundEffect2D, 0.2f);
                m_groamSoundTime = m_groamTimer;
            }
        }

        public void Damage(float damage) {
            if (!m_photonView.IsMine) return;
            if (m_currentHealth <= 0) return;
            onHurt?.Invoke(gameObject);
            m_currentHealth -= damage;
            UpdateLife();
        }

        public void UpdateLife() {
            if (!m_photonView.IsMine) return;
            health.fillAmount = m_currentHealth / characterData.MaxHealth;
            var random = Random.Range(0, characterData.HurtSound.Count);
            AudioController.Instance.Play(characterData.HurtSound[random], AudioController.SoundType.SoundEffect2D);
            AudioController.Instance.Play(HitSound, AudioController.SoundType.SoundEffect2D);
            if (!(m_currentHealth <= 0)) return;
            
            AudioController.Instance.Play(characterData.DeadSound, AudioController.SoundType.SoundEffect2D, 0.2f);
            m_currentHealth = 0;
            healthCanvas.SetActive(false);
            Destroy(this);
            onDeath?.Invoke(gameObject);
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(m_currentHealth);
                stream.SendNext(health.fillAmount);
            }
            else {
                m_currentHealth = (float)stream.ReceiveNext();
                health.fillAmount = (float) stream.ReceiveNext();
            }
        }
    }
}