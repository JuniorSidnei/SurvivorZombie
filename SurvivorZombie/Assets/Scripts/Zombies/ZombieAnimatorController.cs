using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SurvivorZombies.Zombies {
    
    public class ZombieAnimatorController : MonoBehaviour {
        
        public TargetSeeker targetSeeker;
        private Animator m_animator;
        private PhotonView m_photonView;
        
        
        private void Awake() {
            m_animator = GetComponent<Animator>();
            m_photonView = GetComponentInParent<PhotonView>();
            ZombieConstitution.onHurt += OnHurtAnimation;
            ZombieConstitution.onDeath += OnDeathAnimation;
        }

        private void OnHurtAnimation() {
            if (!m_photonView.IsMine) return;
            m_animator.CrossFade("zombie_hurt", 0.2f);
        }

        private void OnDeathAnimation(GameObject zombieObject) {
            if (!m_photonView.IsMine) return;
            m_animator.CrossFade("zombie_dying", 0.2f);
        }
        
        private void Update() {
            if (!m_photonView.IsMine) return;
            m_animator.SetFloat("deltaMovement", targetSeeker.Velocity.magnitude);
        }
    }
}