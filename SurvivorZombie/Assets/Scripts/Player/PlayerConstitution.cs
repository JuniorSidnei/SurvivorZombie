using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Player.Animations;
using SurvivorZombies.Utils;
using SurvivorZombies.Utils.Sound;
using UnityEngine;

namespace SurvivorZombies.Player {

    public class PlayerConstitution : CharacterConstitution, IDamagable, IPunObservable {
        public PlayerAnimatorController PlayerAnimator;
        
        public delegate void OnCharacterDeath(GameObject playerObject);
        public static event OnCharacterDeath onCharacterDeath;
        
        public void Damage(float damage) {
            if (!PhotonView.IsMine) return;
            if (CurrentHealth <= 0) return;
            PlayerAnimator.OnHurtAnimation();
            CurrentHealth -= damage;
            UpdateLife();
        }

        public void UpdateLife() {
            if (!PhotonView.IsMine) return;
            GameManager.Instance.UpdateLife(CurrentHealth / characterData.MaxHealth);
            var random = Random.Range(0, characterData.HurtSound.Count);
            AudioController.Instance.Play(characterData.HurtSound[random], AudioController.SoundType.SoundEffect2D, 0.2f);
            if (!(CurrentHealth <= 0)) return;
            
            AudioController.Instance.Play(characterData.DeadSound, AudioController.SoundType.SoundEffect2D, 0.2f);
            CurrentHealth = 0;
            onCharacterDeath?.Invoke(gameObject);
            GameManager.Instance.EndGame();
            PlayerAnimator.OnDeathAnimation();
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(CurrentHealth);
            }
            else {
                CurrentHealth = (float) stream.ReceiveNext();
            }
        }
    }
}