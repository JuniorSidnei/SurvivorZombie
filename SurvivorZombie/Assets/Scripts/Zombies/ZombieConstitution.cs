using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using SurvivorZombies.Utils;
using SurvivorZombies.Utils.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivorZombies.Zombies {

    public class ZombieConstitution : CharacterConstitution, IDamagable, IPunObservable {
        public List<AudioClip> GroamsSounds;
        public AudioClip HitSound;
        public ZombieAnimatorController ZombieAnimator;
        private float m_groamTimer;
        private float m_groamSoundTime;
        
        private void Start() {
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
            if (!PhotonView.IsMine) return;
            if (CurrentHealth <= 0) return;
            ZombieAnimator.OnHurtAnimation();
            CurrentHealth -= damage;
            UpdateLife();
        }

        public void UpdateLife() {
            if (!PhotonView.IsMine) return;
            health.DOFillAmount(CurrentHealth / characterData.MaxHealth, 0.2f);
            var random = Random.Range(0, characterData.HurtSound.Count);
            AudioController.Instance.Play(characterData.HurtSound[random], AudioController.SoundType.SoundEffect2D);
            AudioController.Instance.Play(HitSound, AudioController.SoundType.SoundEffect2D);
            if (!(CurrentHealth <= 0)) return;
            
            AudioController.Instance.Play(characterData.DeadSound, AudioController.SoundType.SoundEffect2D, 0.2f);
            GameManager.Instance.UpdateScore();
            CurrentHealth = 0;
            healthCanvas.SetActive(false);
            Destroy(this);
            ZombieAnimator.OnDeathAnimation();
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(CurrentHealth);
                stream.SendNext(health.fillAmount);
            }
            else {
                CurrentHealth = (float) stream.ReceiveNext();
                health.fillAmount = (float) stream.ReceiveNext();
            }
        }
        
    }
}